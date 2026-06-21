/*
 * @brief Task Ranking Resolver
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 * @date 2026-06-20
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace multichatdotnet.Helpers
{
    public enum TaskTypeEnum
    {
        light_coding,
        deep_coding,
        research,
        technical_writing,
        fiction_writing,
    }


    internal class FamilyRule
    {
        public Regex Pattern { get; set; }
        public int Priority { get; set; } // higher wins on overlapping matches
        public Dictionary<TaskTypeEnum, int> Scores { get; set; }
    }

    public static class TaskRankingResolver
    {
        // ---- Fallback tiers, used only when no family rule matches ----
        private static readonly Dictionary<TaskTypeEnum, int> FallbackFlagship = Score(6, 6, 6, 6, 6);
        private static readonly Dictionary<TaskTypeEnum, int> FallbackBudget = Score(4, 3, 4, 5, 5);

        // ---- Known model families. Add new entries here as providers ship new models. ----
        // Priority resolves overlaps (e.g. a generic "qwen" rule vs a specific "qwen-coder" rule).
        private static readonly List<FamilyRule> Rules = new List<FamilyRule>
        {
            new FamilyRule
            {
                Pattern = new Regex(@"claude-(opus|sonnet)-4", RegexOptions.IgnoreCase),
                Priority = 100,
                Scores = Score(light: 9, deep: 10, research: 9, techWriting: 9, fiction: 8),
            },
            new FamilyRule
            {
                Pattern = new Regex(@"claude-haiku", RegexOptions.IgnoreCase),
                Priority = 100,
                Scores = Score(light: 8, deep: 6, research: 6, techWriting: 7, fiction: 6),
            },
            new FamilyRule
            {
                Pattern = new Regex(@"qwen2?\.?5?.*(72b|coder)", RegexOptions.IgnoreCase),
                Priority = 90,
                Scores = Score(light: 8, deep: 8, research: 6, techWriting: 6, fiction: 5),
            },
            new FamilyRule
            {
                Pattern = new Regex(@"qwen", RegexOptions.IgnoreCase),
                Priority = 60, // generic qwen catch-all, lower priority than the coder variant above
                Scores = Score(light: 6, deep: 6, research: 5, techWriting: 5, fiction: 5),
            },
            new FamilyRule
            {
                Pattern = new Regex(@"llama-?3\.?[13].*70b", RegexOptions.IgnoreCase),
                Priority = 80,
                Scores = Score(light: 7, deep: 6, research: 6, techWriting: 7, fiction: 7),
            },
            new FamilyRule
            {
                Pattern = new Regex(@"llama", RegexOptions.IgnoreCase),
                Priority = 50,
                Scores = Score(light: 5, deep: 4, research: 5, techWriting: 6, fiction: 6),
            },
            new FamilyRule
            {
                Pattern = new Regex(@"deepseek-r1", RegexOptions.IgnoreCase),
                Priority = 95,
                Scores = Score(light: 7, deep: 9, research: 8, techWriting: 6, fiction: 5),
            },
            new FamilyRule
            {
                Pattern = new Regex(@"deepseek", RegexOptions.IgnoreCase),
                Priority = 55,
                Scores = Score(light: 6, deep: 7, research: 6, techWriting: 5, fiction: 5),
            },
            new FamilyRule
            {
                Pattern = new Regex(@"gemini.*(2\.5-)?pro", RegexOptions.IgnoreCase),
                Priority = 95,
                Scores = Score(light: 8, deep: 8, research: 9, techWriting: 8, fiction: 7),
            },
            new FamilyRule
            {
                Pattern = new Regex(@"gemini.*flash", RegexOptions.IgnoreCase),
                Priority = 85,
                Scores = Score(light: 7, deep: 6, research: 6, techWriting: 7, fiction: 6),
            },
            new FamilyRule
            {
                Pattern = new Regex(@"gpt-4o|gpt-4\.1|gpt-5", RegexOptions.IgnoreCase),
                Priority = 90,
                Scores = Score(light: 8, deep: 8, research: 8, techWriting: 8, fiction: 8),
            },
            new FamilyRule
            {
                Pattern = new Regex(@"mistral.*large", RegexOptions.IgnoreCase),
                Priority = 80,
                Scores = Score(light: 7, deep: 6, research: 6, techWriting: 6, fiction: 6),
            },
        };

        /// <summary>
        /// Resolves task-fitness scores (1-10) for a given model API id.
        /// Highest-priority matching family rule wins; falls back to a size-inferred tier if nothing matches.
        /// </summary>
        public static Dictionary<TaskTypeEnum, int> Resolve(string modelId)
        {
            if (string.IsNullOrWhiteSpace(modelId))
                return FallbackBudget;

            FamilyRule best = null;
            foreach (var rule in Rules)
            {
                if (rule.Pattern.IsMatch(modelId) && (best == null || rule.Priority > best.Priority))
                    best = rule;
            }

            return best?.Scores ?? InferFallbackTier(modelId);
        }

        // Crude safety net for completely unrecognized models: guess flagship vs budget
        // tier from a "NNb" param-count token in the id (e.g. "70b", "8b"). Not a real ranking.
        private static Dictionary<TaskTypeEnum, int> InferFallbackTier(string modelId)
        {
            var sizeMatch = Regex.Match(modelId, @"(\d+)\s*b\b", RegexOptions.IgnoreCase);
            int paramsB = sizeMatch.Success ? int.Parse(sizeMatch.Groups[1].Value) : 0;
            return paramsB >= 32 ? FallbackFlagship : FallbackBudget;
        }

        private static Dictionary<TaskTypeEnum, int> Score(int light, int deep, int research, int techWriting, int fiction)
        {
            return new Dictionary<TaskTypeEnum, int>
            {
                { TaskTypeEnum.light_coding, light },
                { TaskTypeEnum.deep_coding, deep },
                { TaskTypeEnum.research, research },
                { TaskTypeEnum.technical_writing, techWriting },
                { TaskTypeEnum.fiction_writing, fiction },
            };
        }
    }
}
