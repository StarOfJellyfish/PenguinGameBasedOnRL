using UnityEngine;
//0.15 needs sensor
//using MLAgents.Sensors;
using MLAgents;

public class PenguinAgent : Agent
{
    ///=====================================定义变量==================================================
    ///添加公共变量以跟踪企鹅代理以及心脏和反流鱼类的预制件的移动和转动速度。
    ///
    [Tooltip("How fast the agent moves forward")]
    public float moveSpeed = 5f;

    [Tooltip("How fast the agent turns")]
    public float turnSpeed = 180f;

    [Tooltip("Prefab of the heart that appears when the baby is fed")]
    public GameObject heartPrefab;

    [Tooltip("Prefab of the regurgitated fish that appears when the baby is fed")]
    public GameObject regurgitatedFishPrefab;

    ///添加私有变量以跟踪情况。
    private PenguinArea penguinArea;
   // private PenguinAcademy penguinAcademy;
    new private Rigidbody rigidbody;
    private GameObject baby;

    private bool isFull; // If true, penguin has a full stomach
    private float feedRadius = 0f;

    ///=====================================重写InitializeAgent==================================================
    ///agent唤醒时，会自动调用一次InitializeAgent（）。
    ///每次重置代理时都不会调用它，这就是为什么有单独的ResetAgent（）函数的原因。我们将使用它在场景中查找一些对象。
    ///

    /// <summary>
    /// Initial setup, called when the agent is enabled
    /// </summary>
    /// AgentAction（）是代理程序接收和响应命令的位置。这些命令可能源自神经网络或人类玩家，但是此功能将它们视为相同。
    /// vectorAction参数是一个数字值数组，与代理应采取的操作相对应。
    /// 对于此项目，我们使用“离散”操作，这意味着每个整数值（例如0、1、2，…）对应于一个选择。
    /// 另一种选择是“连续”动作，它允许在-1和+1之间选择任何小数值（例如-.7、0.23，.4等）。离散动作一次只能选择一个选项，而不会中间出现。
    /// 在这种情况下：
    ///vectorAction[0] 可以为0或1，指示是保留在原位（0）还是以全速前进（1）。
    ///vectorAction[1] 可以是0、1或2，指示是不转（0），沿负方向（1）还是沿正方向（2）。
    /// 经过训练的神经网络实际上并不了解这些动作的作用。它只知道，当以某种方式看到环境时，某些动作往往会带来更多的奖励积分。
    /// 这就是为什么在此脚本的后面部分创建对环境的有效观察非常重要的原因。
    ///解释了矢量动作之后，AgentAction（）函数将应用移动和旋转，然后添加少量的负奖励。
    ///这种小的负面奖励会鼓励代理尽快完成任务。
    ///在这种情况下，每5,000个步骤将获得-1 / 5000的奖励。
    ///如果企鹅提前完成（例如，以3,000步为单位），则从此代码行添加的负奖励将为-3000 / 5000 = -0.6。
    ///如果企鹅走了所有5,000步，则总负回报将为-5000 / 5000 = -1。
    public override void InitializeAgent()
    {
        base.InitializeAgent();
        penguinArea = GetComponentInParent<PenguinArea>();
       // penguinAcademy = FindObjectOfType<PenguinAcademy>();
        baby = penguinArea.penguinBaby;
        rigidbody = GetComponent<Rigidbody>();
    }



    ///======================================================================================================
    /// <summary>
    /// Perform actions based on a vector of numbers
    /// </summary>
    /// <param name="vectorAction">The list of actions to take</param>
    /// Heuristic（）函数允许在没有神经网络的情况下控制代理。该功能将通过键盘读取人类演奏者的输入，将其转换为动作，并返回这些动作的列表。
    ///  在我们的项目中：
    ///  默认的forwardAction将为0，但是如果播放器在键盘上按下“ W”，则此值将设置为1。
    ///  默认的turnAction将为0，但是如果玩家按下键盘上的“ A”或“ D”，则该值将分别设置为1或2以向左或向右转。
    public override void AgentAction(float[] vectorAction)
    {
        // Convert the first action to forward movement
        float forwardAmount = vectorAction[0];

        // Convert the second action to turning left or right
        float turnAmount = 0f;
        if (vectorAction[1] == 1f)
        {
            turnAmount = -1f;
        }
        else if (vectorAction[1] == 2f)
        {
            turnAmount = 1f;
        }

        // Apply movement
        rigidbody.MovePosition(transform.position + transform.forward * forwardAmount * moveSpeed * Time.fixedDeltaTime);
        transform.Rotate(transform.up * turnAmount * turnSpeed * Time.fixedDeltaTime);

        // Apply a tiny negative reward every step to encourage action
        if (maxStep > 0) AddReward(-1f / maxStep);
    }

    /// <summary>
    /// Read inputs from the keyboard and convert them to a list of actions.
    /// This is called only when the player wants to control the agent and has set
    /// Behavior Type to "Heuristic Only" in the Behavior Parameters inspector.
    /// </summary>
    /// <returns>A vectorAction array of floats that will be passed into <see cref="AgentAction(float[])"/></returns>
    //    The Heuristic() function allows control of the agent without a neural network.This function will read inputs from the human player via the keyboard, convert them into actions, and return a list of those actions.In our project:


    // The default forwardAction will be 0, but if the player presses 'W' on the keyboard, this value will be set to 1.
    //The default turnAction will be 0, but if the player presses 'A' or 'D' on the keyboard, the value will be set to 1 or 2 respectively to turn left or right.


    // Override the Heuristic() function.

    /// <summary>
    /// Read inputs from the keyboard and convert them to a list of actions.
    /// This is called only when the player wants to control the agent and has set
    /// Behavior Type to "Heuristic Only" in the Behavior Parameters inspector.
    /// </summary>
    /// <returns>A vectorAction array of floats that will be passed into <see cref="AgentAction(float[])"/></returns>
    public override float[] Heuristic()
    {
        float forwardAction = 0f;
        float turnAction = 0f;
        if (Input.GetKey(KeyCode.W))
        {
            // move forward
            forwardAction = 1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            // turn left
            turnAction = 1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            // turn right
            turnAction = 2f;
        }

        // Put the actions into an array and return
        return new float[] { forwardAction, turnAction };
    }
    ///当代理完成为婴儿喂完所有鱼后，基本的Agent类会自动调用AgentReset（）函数。我们将使用它清空企鹅的腹部并重置该区域。
    /// <summary>
    /// Reset the agent and area
    /// </summary>
    // <summary>
    /// Reset the agent and area
    /// </summary>
    public override void AgentReset()
    {
        isFull = false;
        penguinArea.ResetArea();
        feedRadius = Academy.Instance.FloatProperties.GetPropertyWithDefault("feed_radius", 0f);
    }

    /// <summary>
    /// Collect all non-Raycast observations
    /// </summary>
    /// <summary>
    /// Collect all non-Raycast observations
    /// </summary>
    // 0.15
    //public override void CollectObservations(VectorSensor sensor)

    //{
    //    // Whether the penguin has eaten a fish (1 float = 1 value)
    //    sensor.AddObservation(isFull);

    //    // Distance to the baby (1 float = 1 value)
    //    sensor.AddObservation(Vector3.Distance(baby.transform.position, transform.position));

    //    // Direction to baby (1 Vector3 = 3 values)
    //    sensor.AddObservation((baby.transform.position - transform.position).normalized);

    //    // Direction penguin is facing (1 Vector3 = 3 values)
    //    sensor.AddObservation(transform.forward);

    //    // 1 + 1 + 3 + 3 = 8 total values
    //}

    ////0.14
    public override void CollectObservations()
    {
        // Whether the penguin has eaten a fish (1 float = 1 value)
        AddVectorObs(isFull);

        // Distance to the baby (1 float = 1 value)
        AddVectorObs(Vector3.Distance(baby.transform.position, transform.position));

        // Direction to baby (1 Vector3 = 3 values)
        AddVectorObs((baby.transform.position - transform.position).normalized);

        // Direction penguin is facing (1 Vector3 = 3 values)
        AddVectorObs(transform.forward);

        // 1 + 1 + 3 + 3 = 8 total values
    }

    private void FixedUpdate()
    {
        // Request a decision every 5 steps. RequestDecision() automatically calls RequestAction(),
        // but for the steps in between, we need to call it explicitly to take action using the results
        // of the previous decision
        if (GetStepCount() % 5 == 0)
        {
            RequestDecision();
        }
        else
        {
            RequestAction();
        }

        // Test if the agent is close enough to to feed the baby
        if (Vector3.Distance(transform.position, baby.transform.position) < feedRadius)
        {
            // Close enough, try to feed the baby
            RegurgitateFish();
        }
    }

    /// <summary>
    /// When the agent collides with something, take action
    /// </summary>
    /// <param name="collision">The collision info</param>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("fish"))
        {
            // Try to eat the fish
            EatFish(collision.gameObject);
        }
        else if (collision.transform.CompareTag("baby"))
        {
            // Try to feed the baby
            RegurgitateFish();
        }
    }

    /// <summary>
    /// Check if agent is full, if not, eat the fish and get a reward
    /// </summary>
    /// <param name="fishObject">The fish to eat</param>
    private void EatFish(GameObject fishObject)
    {
        if (isFull) return; // Can't eat another fish while full
        isFull = true;

        penguinArea.RemoveSpecificFish(fishObject);

        AddReward(1f);
    }

    /// <summary>
    /// Check if agent is full, if yes, feed the baby
    /// </summary>
    private void RegurgitateFish()
    {
        if (!isFull) return; // Nothing to regurgitate
        isFull = false;

        // Spawn regurgitated fish
        GameObject regurgitatedFish = Instantiate<GameObject>(regurgitatedFishPrefab);
        regurgitatedFish.transform.parent = transform.parent;
        regurgitatedFish.transform.position = baby.transform.position;
        Destroy(regurgitatedFish, 4f);

        // Spawn heart
        GameObject heart = Instantiate<GameObject>(heartPrefab);
        heart.transform.parent = transform.parent;
        heart.transform.position = baby.transform.position + Vector3.up;
        Destroy(heart, 4f);

        AddReward(1f);

        if (penguinArea.FishRemaining <= 0)
        {
            Done();
        }
    }
}