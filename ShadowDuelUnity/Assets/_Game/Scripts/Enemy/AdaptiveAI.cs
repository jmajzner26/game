using UnityEngine;
using System.Collections.Generic;

namespace ShadowDuel.Enemy
{
    /// <summary>
    /// Advanced AI that learns and adapts to player patterns
    /// </summary>
    public class AdaptiveAI : EnemyAI
    {
        [Header("Learning Settings")]
        [SerializeField] private int memorySize = 20;
        [SerializeField] private float learningRate = 0.1f;
        [SerializeField] private float adaptationThreshold = 0.3f;

        // Pattern tracking
        private List<PlayerAction> recentPlayerActions = new List<PlayerAction>();
        private Dictionary<AttackPattern, int> patternFrequency = new Dictionary<AttackPattern, int>();
        private Dictionary<Vector2, float> dodgeDirections = new Dictionary<Vector2, float>();

        // Adaptive behaviors
        private float parryChance = 0.3f;
        private float dodgeChance = 0.2f;
        private float counterWindow = 0.5f;

        private float attackPredictionAccuracy = 0f;

        private struct PlayerAction
        {
            public Vector3 position;
            public Vector3 direction;
            public bool isHeavy;
            public float timestamp;
        }

        private struct AttackPattern
        {
            public Vector2 direction;
            public bool isHeavy;
            public float cooldownAfter;

            public override int GetHashCode()
            {
                return direction.GetHashCode() ^ isHeavy.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (obj is AttackPattern other)
                {
                    return direction == other.direction && isHeavy == other.isHeavy;
                }
                return false;
            }
        }

        protected override void UpdateAI()
        {
            base.UpdateAI();

            // Track player actions
            if (target)
            {
                TrackPlayerAction();
            }

            // Learn from patterns
            if (recentPlayerActions.Count >= memorySize)
            {
                AnalyzePatterns();
                AdaptBehavior();
            }
        }

        private void TrackPlayerAction()
        {
            Vector3 playerDirection = target.transform.forward;
            bool isHeavy = false; // Would need to detect from animations or state

            recentPlayerActions.Add(new PlayerAction
            {
                position = target.transform.position,
                direction = playerDirection,
                isHeavy = isHeavy,
                timestamp = Time.time
            });

            // Keep only recent actions
            if (recentPlayerActions.Count > memorySize)
            {
                recentPlayerActions.RemoveAt(0);
            }
        }

        private void AnalyzePatterns()
        {
            // Reset pattern frequency
            patternFrequency.Clear();

            // Count pattern occurrences
            for (int i = 0; i < recentPlayerActions.Count - 1; i++)
            {
                PlayerAction current = recentPlayerActions[i];
                PlayerAction next = recentPlayerActions[i + 1];

                AttackPattern pattern = new AttackPattern
                {
                    direction = new Vector2(current.direction.x, current.direction.z),
                    isHeavy = next.isHeavy,
                    cooldownAfter = next.timestamp - current.timestamp
                };

                if (patternFrequency.ContainsKey(pattern))
                {
                    patternFrequency[pattern]++;
                }
                else
                {
                    patternFrequency[pattern] = 1;
                }
            }

            // Calculate most common patterns
            int maxFreq = 0;
            foreach (var freq in patternFrequency.Values)
            {
                if (freq > maxFreq)
                {
                    maxFreq = freq;
                }
            }

            // If patterns are strong, increase adaptation
            if (maxFreq > memorySize * adaptationThreshold)
            {
                attackPredictionAccuracy += learningRate;
                attackPredictionAccuracy = Mathf.Clamp01(attackPredictionAccuracy);
            }
        }

        private void AdaptBehavior()
        {
            // Increase parry chance if player is predictable
            parryChance = 0.3f + (attackPredictionAccuracy * 0.4f);

            // Increase dodge chance if player uses heavy attacks often
            float heavyAttackRatio = CountHeavyAttacks() / (float)recentPlayerActions.Count;
            dodgeChance = 0.2f + (heavyAttackRatio * 0.3f);

            // Adjust counter window based on learning
            counterWindow = 0.5f - (attackPredictionAccuracy * 0.2f);
        }

        private int CountHeavyAttacks()
        {
            int count = 0;
            foreach (var action in recentPlayerActions)
            {
                if (action.isHeavy)
                    count++;
            }
            return count;
        }

        protected override void MakeDecision()
        {
            if (Time.time - lastDecisionTime < decisionInterval)
                return;

            lastDecisionTime = Time.time;

            // Make adaptive decisions
            base.MakeDecision();
        }

        protected override void HandleAttack()
        {
            base.HandleAttack();

            // Try to counter based on prediction
            if (attackPredictionAccuracy > 0.5f && Random.value < parryChance)
            {
                if (IsPredictingAttack())
                {
                    currentState = AIState.Block;
                }
            }

            // Try to dodge heavy attacks
            if (Random.value < dodgeChance && IsPredictingHeavyAttack())
            {
                currentState = AIState.Dodge;
            }
        }

        private bool IsPredictingAttack()
        {
            if (recentPlayerActions.Count < 2)
                return false;

            // Check if pattern suggests incoming attack
            PlayerAction recent = recentPlayerActions[recentPlayerActions.Count - 1];
            float timeSinceLastAction = Time.time - recent.timestamp;

            if (timeSinceLastAction < counterWindow)
            {
                return true;
            }

            return false;
        }

        private bool IsPredictingHeavyAttack()
        {
            if (recentPlayerActions.Count < 3)
                return false;

            // Check for pattern of heavy attacks
            int recentHeavyCount = 0;
            for (int i = recentPlayerActions.Count - 3; i < recentPlayerActions.Count; i++)
            {
                if (recentPlayerActions[i].isHeavy)
                    recentHeavyCount++;
            }

            return recentHeavyCount >= 2;
        }

        /// <summary>
        /// Reset learning when player changes patterns significantly
        /// </summary>
        public void ResetLearning()
        {
            attackPredictionAccuracy *= 0.5f;
            recentPlayerActions.Clear();
            patternFrequency.Clear();
        }

        public float GetPredictionAccuracy() => attackPredictionAccuracy;
    }
}

