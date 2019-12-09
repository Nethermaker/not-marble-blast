using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//Buckle up, this one's gonna be long
//Controls the player gameobject, as well as many, many other things
public class PlayerController : MonoBehaviour {

    //The player needs to reference a lot of different aspects of the game, so here we are with this giant mess
    //Variables marked as public can A) Be accessed from the editor, and B) Be accessed from other scripts
    //Access from the editor is extremely useful when dealing with prefabs (premade objects that can then be instantiated en-masse) like particle effects
    //Otherwise it's usually best to make variables private and assign them within the Start() function

    //This first section is just stuff that had no other home (lists, booleans, etc.)
    private Rigidbody rb;
    private bool isGrounded;
    public bool levelEnded = false;
    public bool isPaused = false;
    private bool levelHasGems = true;
    private string activePowerup = "None";
    private int maxGems = 0;
    private int currentGems = 0;
    private List<GameObject> recentGems = new List<GameObject>();
    public List<GameObject> usedRotators = new List<GameObject>();
    private bool gryoIsActive = false;
    public bool canReset = true;

    //References the main camera's location and the location of the last-touched checkpoint
    public Transform cam;
    private Transform activeCheckpoint;

    //References to the pause menu and menu manager
    public GameObject pauseMenu;
    public GameObject menuManager;

    //These are all values related to the "feel" of the player
    private float speed = 5;
    public float airSpeed = 4f;
    public float groundSpeed = 5f;
    public float turn = 10;
    public float jumpHeight = 5;
    public float superJumpHeight = 20;
    public float superSpeedSpeed = 10;
    public float distanceToGround = 1;

    //References to in-game UI objects
    public Text powerupText;
    public Text gemText;
    public Text alertText;

    //These are all particle effects
    public Transform burst;
    public Transform trail;
    public Transform flight;
    public Transform checkpointParticles;
    public Transform finishParticles;

    //Not entirely sure how this works, but it's used later so that the player can't jump off of non-solid colliders
    private int powerUpMask = 1 << 9;

    //Start() is called once on level load
    private void Start()
    {
        //Gets the player's RigidBody component, which allows it to be moved via physics
        rb = GetComponent<Rigidbody>();
        //Uncaps angular velocity, allowing the ball to spin as fast as it wants to
        rb.maxAngularVelocity = Mathf.Infinity;
        //This function is used to set the powerup text whenever the player picks up or uses a powerup, as well as on level load
        SetPowerupText();
        powerUpMask = ~powerUpMask;

        //First checkpoint is the start
        activeCheckpoint = GameObject.Find("Start").transform;
        //Locks and hides the cursor, keeping it out of the way
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //Finds all gems, counting them
        foreach (GameObject gem in GameObject.FindGameObjectsWithTag("Gem"))
        {
            maxGems++;
            //Was going to disable the gem text if the level had no gems, but this doesn't work for some reason
            if (GameObject.FindGameObjectsWithTag("Gem") == null)
            {
                levelHasGems = false;
            }
        }
        //Same as SetPowerupText()
        SetGemText();

        cam = GameObject.Find("Main Camera").transform;

        //This fixes a bug I found at the absolute last minute
        Physics.gravity = new Vector3(0, -10, 0);
    }

    //FixedUpdate() is called at every physics update (30 times per second by default) and is unaffected by differences in framerate
    //Anything physics related should be put in here
    private void FixedUpdate()
    {
        //Can only control player if the level is not over or paused
        if (!levelEnded && !isPaused)
        {
            //Gets axes of movement (WASD keys by default)
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            //We want movement to be based on camera orientation, but not affected by how far up or down it is angled
            Vector3 cameraRotation = cam.transform.localRotation.eulerAngles;
            cameraRotation.x = 0f;

            //Creates a vector3 based on movement, rotates it by the camera's orientation, then is applied to the player's rigidbody as a force
            Vector3 movement = new Vector3(moveHorizontal, 0f, moveVertical);
            movement = Quaternion.Euler(cameraRotation) * movement;
            rb.AddForce(movement * speed);
            
            
            //Spins the player based on movement (needed to control spin while airborne)
            rb.AddTorque(Quaternion.Euler(Vector3.up * 90) * movement * turn, ForceMode.Acceleration);

            //Sends a ray down, and if it hits a collider that isn't a trigger, then the player can jump
            if (Physics.Raycast(transform.position, Vector3.down, distanceToGround, powerUpMask))
            {
                Debug.DrawRay(transform.position, Vector3.down, Color.yellow);
                isGrounded = true;
            }
            else
            {
                Debug.DrawRay(transform.position, Vector3.down, Color.white);
                isGrounded = false;
            }
        }
    }

    //Update() is called at the start of every frame (LateUpdate() is called at the end of every frame)
    private void Update()
    {
        if (!levelEnded && !isPaused)
        {
            //Gets various inputs from the player
            if ((Input.GetKeyDown(KeyCode.Space) && isGrounded) || (Input.GetKey(KeyCode.Space) && isGrounded))
            {
                Jump();
            }

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                UsePowerUp();
            }

            if (Input.GetKeyDown(KeyCode.R) && canReset)
            {
                Reset();
            }

            //These are things related to the "feel" of movement
            if (isGrounded)
            {
                rb.drag = 0.4f;
                rb.angularDrag = 0.4f;
                speed = groundSpeed;
            }
            else
            {
                rb.drag = 0f;
                rb.angularDrag = 0f;
                speed = airSpeed;
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused == false && !levelEnded)
            {
                Pause();
            }
            else if (isPaused == true && !levelEnded)
            {
                Unpause();
            }
        }
    }

    //Called when the player enters a trigger, and gives a reference to said trigger collider
    private void OnTriggerEnter(Collider other)
    {
        //When the player touches a powerup, if the powerup is active, then the player gets that powerup and deactivates it
        if (other.gameObject.CompareTag("Power Up"))
        {
            GameObject powerUp = other.gameObject;
            Powerup powerUpScript = powerUp.GetComponent<Powerup>();
            if (powerUpScript.isActive)
            {
                powerUpScript.Deactivate();
                activePowerup = powerUp.name;
                SetPowerupText();
            }
        }
        //Upon touching a gem, disable it, add it to a list so it can be reenabled if the player resets, and increment the number of gems collected
        else if (other.gameObject.CompareTag("Gem"))
        {
            currentGems++;
            SetGemText();
            other.gameObject.SetActive(false);
            recentGems.Add(other.gameObject);
        }
        //When reaching a checkpoint, set it as the active checkpoint, and play a particle effect if it isn't already the active checkpoint
        else if (other.gameObject.CompareTag("Checkpoint"))
        {
            if (other.gameObject != activeCheckpoint.gameObject)
            {
                Instantiate(checkpointParticles, other.gameObject.transform.position, Quaternion.Euler(-90, 0, 0));
            }
            activeCheckpoint = other.gameObject.transform;
            //Clearing the list of recently collected gems essentially "saves" them as being permanently collected
            recentGems.Clear();
            usedRotators.Clear();
        }
        //End the level when the player reaches the finish
        else if (other.gameObject.CompareTag("Finish"))
        {
            EndLevel();
        }
    }

    //Called when the player exits a trigger
    private void OnTriggerExit(Collider other)
    {
        //If the player leaves a level's boundaries
        if (other.gameObject.CompareTag("Level Bounds"))
        {
            //Starts the OutOfBounds coroutine (explained further below)
            StartCoroutine("OutOfBounds");
        }
        else if (other.gameObject.CompareTag("Finish"))
        {
            alertText.text = "";
        }
    }

    //Jumping is as simple as setting vertical velocity to a certain value
    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, jumpHeight, rb.velocity.z);
    }

    void UsePowerUp()
    {
        if (activePowerup == "Super Jump")
        {
            //Super Jump actually adds to the player's vertical velocity (rather than setting it) since it can be used anywhere
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y + superJumpHeight, rb.velocity.z);
            activePowerup = "None";
            SetPowerupText();
            //Plays the super jump particle effect
            Transform effect = Instantiate(burst, transform.position, Quaternion.Euler(90,0,0));
            Destroy(effect.gameObject, effect.GetComponent<ParticleSystem>().main.duration);
        }
        else if (activePowerup == "Super Speed")
        {
            //The super speed multiplies the player's non-vertical velocity by a certain amount
            rb.velocity = new Vector3(rb.velocity.x * superSpeedSpeed, rb.velocity.y, rb.velocity.z * superSpeedSpeed);
            activePowerup = "None";
            SetPowerupText();
            //Plays the super speed particle effect
            Transform effect = Instantiate(trail, transform);
            Destroy(effect.gameObject, effect.GetComponent<ParticleSystem>().main.duration);
        }
        else if (activePowerup == "Gyrocopter")
        {
            //The gyrocopter functions differently than the other two, so it uses a coroutine
            StartCoroutine("Gyrocopter");
        }
    }

    //Coroutines are identical to functions with one difference: their execution can be paused
    //They can be paused until the next frame by using "yield return null",
    //   paused for an amount of time by using "yield return new WaitForSeconds(time)",
    //   and/or ended early using "yield break"
    //They are extremely useful in a number of cases where otherwise you would have to use a bunch of timers to achieve the same effect
    IEnumerator Gyrocopter()
    {
        //The gyrocopter decreases the player's gravity, allowing "flight"
        if (gryoIsActive == true)
        {
            //If a gyrocopter is already in use, do nothing
            yield break;
        }
        else
        {
            activePowerup = "None";
            SetPowerupText();
            //Creates particle effect and parents it to the player (so it moves with the player)
            gryoIsActive = true;
            Instantiate(flight, transform);
            //Sets gravity level
            Vector3 originalGravity = Physics.gravity;
            Physics.gravity = originalGravity * 0.25f;

            //Waits for 8 seconds
            yield return new WaitForSeconds(8);

            //Sets gravity back to original levels
            Physics.gravity = originalGravity;
            gryoIsActive = false;
        }
    }

    //This should be self-explanatory
    void SetPowerupText()
    {
        powerupText.text = "Current Powerup: " + activePowerup;
    }

    //This should also be self-explanatory
    void SetGemText()
    {
        if (levelHasGems)
        {
            gemText.text = "Gem Count: " + currentGems + " / " + maxGems;
        }
        else
        {
            gemText.text = "";
            gemText.gameObject.SetActive(false);
        }
    }

    //Called when the player resets manually or falls out of bounds
    void Reset()
    {
        //Reenable all used rotators
        foreach (GameObject rotator in usedRotators)
        {
            rotator.GetComponent<Rotator>().isActive = true;
        }

        //Set player velocity to zero
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        //Sets the player's position to a point slightly above the checkpoint (which is held by an empty child gameobject)
        foreach (Transform child in activeCheckpoint.transform)
        {
            transform.position = child.transform.position;
        }

        activePowerup = "None";
        SetPowerupText();

        //Re-enables all gems collected since the last checkpoint was reached
        foreach (GameObject gem in recentGems)
        {
            gem.SetActive(true);
            currentGems--;
        }

        recentGems.Clear();
        SetGemText();
    }

    //Called upon level end
    void EndLevel()
    {
        //Only continues if player has all gems
        if (currentGems == maxGems)
        {
            //Freezes the player, sets levelEnded to true (preventing input), unlocks the cursor, tells the menumanager that the level is ended, and puts particles at the finish
            rb.constraints = RigidbodyConstraints.FreezePosition;
            levelEnded = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            menuManager.GetComponent<LevelMenuManager>().LevelEnd();
            Instantiate(finishParticles, GameObject.Find("Finish").transform.position, Quaternion.Euler(-90, 0, 0));
        }
        else
        {
            alertText.text = "You don't have all the gems!";
        }
    }

    //Called when game is paused, this sets the timescale of the game to 0 (freezing almost everything) and activates the pause menu
    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        pauseMenu.SetActive(true);
    }

    //Does the opposite of Pause()
    public void Unpause()
    {
        isPaused = false;
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pauseMenu.SetActive(false);
    }

    //Gets the name of the next level to be loaded (held by the Finish script on the finish gameobject) and loads it
    //Called by the end of level menu
    void LoadNextLevel()
    {
        GameObject finish = GameObject.FindGameObjectWithTag("Finish");
        string nextLevel = finish.GetComponent<Finish>().nextLevel;
        SceneManager.LoadScene(nextLevel);
    }

    //This coroutine creates a neat little effect when the player goes out of bounds, and resets the player
    IEnumerator OutOfBounds()
    {
        CameraController camController = cam.GetComponent<CameraController>();
        camController.positionLocked = true;
        alertText.text = "Out of bounds!";
        canReset = false;
        yield return new WaitForSeconds(2);
        alertText.text = "";
        camController.positionLocked = false;
        Reset();
        canReset = true;
    }
}
