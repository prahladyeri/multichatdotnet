/*
 * @brief Builds a sorted, grid-bindable ranking of models for a chosen TaskTypeEnum.
 *        Used in the wizard step where the user picks a task and sees best-fit models.
 *
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 * @date 2026-06-20
 */
using System.Collections.Generic;
using System.Linq;

namespace multichatdotnet.Helpers
{
    // Flat, grid-friendly row. Properties (not fields) so DataGridView/BindingSource
    // can auto-generate columns via reflection.
    public class ModelTaskRankingRow
    {
        public string ModelId { get; set; }
        public string DisplayName { get; set; }
        public string ProviderId { get; set; }
        public int Score { get; set; }
        public bool IsFreeTier { get; set; }
        public decimal CostPerMillionInputTokens { get; set; }
    }

    public static class ModelRankingHelper
    {
        /// <summary>
        /// Ranks the given models for a single task, best-first. Ties broken by lower input cost
        /// (cheap models win ties — adjust if you'd rather break ties by context window or something else).
        /// </summary>
        public static List<ModelTaskRankingRow> RankModelsForTask(IEnumerable<ModelInfo> models, TaskTypeEnum task)
        {
            return models
                .Select(m => new ModelTaskRankingRow
                {
                    ModelId = m.Id,
                    DisplayName = m.DisplayName,
                    ProviderId = m.ProviderId,
                    Score = m.TaskRanking.TryGetValue(task, out var s) ? s : 0,
                    IsFreeTier = m.IsFreeTier,
                    CostPerMillionInputTokens = m.CostPerMillionInputTokens,
                })
                .OrderByDescending(r => r.Score)
                .ThenBy(r => r.CostPerMillionInputTokens)
                .ToList();
        }
    }
}