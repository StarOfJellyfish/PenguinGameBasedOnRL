using UnityEngine;

public class Fish : MonoBehaviour
{
    [Tooltip("The swim speed")]
    public float fishSpeed;

    private float randomizedSpeed = 0f;
    private float nextActionTime = -1f;
    private Vector3 targetPosition;
    ///接下来，我们将添加游泳功能。在任何给定的更新中，鱼将选择新的速度和目的地，或朝其当前目的地移动。
    ///  该采取新的行动了，这条鱼将：
    ///   在平均鱼速的50％到150％之间选择一个新的随机速度。
    ///    选择一个新的随机目标位置（在水中）游泳。
    ///    旋转鱼以面对目标。
    ///   计算到达那里所需的时间。
    ///    否则，鱼将朝目标移动，并确保它不会游过目标。
    /// <summary>
    /// Called every timestep
    /// </summary>
    private void FixedUpdate()
    {
        if (fishSpeed > 0f)
        {
            Swim();
        }
    }


    /// <summary>
    /// Swim between random positions
    /// </summary>
    private void Swim()
    {
        // If it's time for the next action, pick a new speed and destination
        // Else, swim toward the destination
        if (Time.fixedTime >= nextActionTime)
        {
            // Randomize the speed
            randomizedSpeed = fishSpeed * UnityEngine.Random.Range(.5f, 1.5f);

            // Pick a random target
            targetPosition = PenguinArea.ChooseRandomPosition(transform.parent.position, 100f, 260f, 2f, 13f);

            // Rotate toward the target
            transform.rotation = Quaternion.LookRotation(targetPosition - transform.position, Vector3.up);

            // Calculate the time to get there
            float timeToGetThere = Vector3.Distance(transform.position, targetPosition) / randomizedSpeed;
            nextActionTime = Time.fixedTime + timeToGetThere;
        }
        else
        {
            // Make sure that the fish does not swim past the target
            Vector3 moveVector = randomizedSpeed * transform.forward * Time.fixedDeltaTime;
            if (moveVector.magnitude <= Vector3.Distance(transform.position, targetPosition))
            {
                transform.position += moveVector;
            }
            else
            {
                transform.position = targetPosition;
                nextActionTime = Time.fixedTime;
            }
        }
    }
}