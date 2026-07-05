using UnityEngine; 

namespace Data
{
    [CreateAssetMenu(fileName = "QuizReplicas", menuName = "Quiz/Replicas Keys")]
    public class QuizReplicasSO : ScriptableObject
    {
        public Replicas ReplicasData;
    }
}