﻿using LogAnalyzer.Analyzers.Migration.R2RPlus;
using LogsAnalyzer.Infrastructure.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace LogsAnalyzer.Renderers.WinForms.TreeView {
    public class R2RPlusMigrationTreeViewRenderer : BaseTreeViewRenderer {
        protected R2RPlusMigrationAnalyzer Analyzer;

        public override Type AnalyzerType => typeof(R2RPlusMigrationAnalyzer);

        public override TreeNode Render() {
            var rootResult = new TreeNode {
                Text = $"R2RPlus Migration Analyzer Results ({Analyzer.R2RPlusMigrationResults.Count})"
            };



            foreach (var r in Analyzer.R2RPlusMigrationResults) {
                var resultRootNode = CreateNodeWithCommonContextMenuStrip($"{r.LogId} | {r.RPlusBusinessId} | {r.RBusinessId}");
                resultRootNode.Nodes.Add(CreateNodeWithCommonContextMenuStrip($"Start: {r.StartDate} | End: {r.EndDate}"));
                resultRootNode.Nodes.Add(CreateNode($"Updated business? {r.WasBusinessUpdated}"));
                resultRootNode.Nodes.Add(CreateNode($"Migration all OK? {!r.HadErrors}"));
                resultRootNode.Nodes.Add(CreateNode($"Lines {r.StartLineNumber} to {r.EndLineNumber}"));
                renderInsertedBookings(resultRootNode, r.InsertedBookings);
                renderInsertedProducts(resultRootNode, r.InsertedProducts, r.InsertedInactiveProducts);
                renderInsertedRatePlans(resultRootNode, r.InsertedRatePlans);
                rootResult.Nodes.Add(resultRootNode);
            }

            return rootResult;
        }

        private void renderInsertedRatePlans(TreeNode parentNode, List<InsertedRPlusData> insertedRatePlans) {
            //var failed = insertedRatePlans.Where(b => !b.IsOk).Count();
            //var insertedBookingsNode = CreateNodeWithCommonContextMenuStrip($"Inserted rate plans (Failed: {failed} | Total: {insertedRatePlans.Count})");
            //foreach (var b in insertedRatePlans) {
            //    var ok = b.IsOk ? "OK" : "FAILED";
            //    var msg = string.IsNullOrEmpty(b.StatusMessage) ? "" : $"({b.StatusMessage})";
            //    var node = CreateNodeWithCommonContextMenuStrip($"R Id: {b.RId} | R+ Id: {b.RPlusId} | {ok} {msg}");
            //    insertedBookingsNode.Nodes.Add(node);
            //}
            //parentNode.Nodes.Add(insertedBookingsNode);

            var failed = insertedRatePlans.Where(b => !b.IsOk).Count();
            var insertedSummaryNode = CreateNodeWithCommonContextMenuStrip($"Inserted rate plans (Failed: {failed} | Total: {insertedRatePlans.Count})");
            foreach (var p in insertedRatePlans) {
                var ok = p.IsOk ? "OK" : "FAILED";
                var msg = string.IsNullOrEmpty(p.StatusMessage) ? "" : $"({p.StatusMessage})";
                var ratePlanNode = CreateNodeWithCommonContextMenuStrip($"{p.Name} | {ok}");
                var node = CreateNodeWithCommonContextMenuStrip($"R Id: {p.RId} | R+ Id: {p.RPlusId} | {msg}");
                ratePlanNode.Nodes.Add(node);
                insertedSummaryNode.Nodes.Add(ratePlanNode);
            }
            parentNode.Nodes.Add(insertedSummaryNode);
        }

        private void renderInsertedProducts(TreeNode parentNode, List<InsertedRPlusData> insertedProducts, List<InsertedInactiveProduct> insertedInactiveProducts) {
            var failed = insertedProducts.Where(b => !b.IsOk).Count();
            var summaryText = $"Inserted products (Failed: {failed} | ";
            if (insertedInactiveProducts.Any()) { 
                summaryText += $"Inactive: {insertedInactiveProducts.Count} | ";
            }
            summaryText += $"Total: {insertedProducts.Count + insertedInactiveProducts.Count})";
            var insertedSummaryNode = CreateNodeWithCommonContextMenuStrip(summaryText);
            foreach (var p in insertedInactiveProducts) {
                var productNode = CreateNodeWithCommonContextMenuStrip($"Inactive product for Rate Plan {p.RatePlanName} ({p.RatePlanId})");
                var node = CreateNodeWithCommonContextMenuStrip($"R Id: {p.RId} | R+ Id: {p.RPlusId}");
                productNode.Nodes.Add(node);
                insertedSummaryNode.Nodes.Add(productNode);
            }

            foreach (var p in insertedProducts) {
                var ok = p.IsOk ? "OK" : "FAILED";
                var msg = string.IsNullOrEmpty(p.StatusMessage) ? "" : $"({p.StatusMessage})";
                var productNode = CreateNodeWithCommonContextMenuStrip($"{p.Name} | {ok}");
                var node = CreateNodeWithCommonContextMenuStrip($"R Id: {p.RId} | R+ Id: {p.RPlusId} | {msg}");
                productNode.Nodes.Add(node);
                insertedSummaryNode.Nodes.Add(productNode);
            }
            parentNode.Nodes.Add(insertedSummaryNode);
        }

        private void renderInsertedBookings(TreeNode parentNode, List<InsertedRPlusData> insertedBookings) {
            var failed = insertedBookings.Where(b => !b.IsOk).Count();
            var insertedBookingsNode = CreateNodeWithCommonContextMenuStrip($"Inserted bookings (Failed: {failed} | Total: {insertedBookings.Count})");
            foreach (var b in insertedBookings) {
                var ok = b.IsOk ? "OK" : "FAILED";
                var msg = string.IsNullOrEmpty(b.StatusMessage) ? "" : $"({b.StatusMessage})";
                var node = CreateNodeWithCommonContextMenuStrip($"R Id: {b.RId} | R+ Id: {b.RPlusId} | {ok} {msg}");
                insertedBookingsNode.Nodes.Add(node);
            }
            parentNode.Nodes.Add(insertedBookingsNode);
        }

        public override void SetAnalyzer(BaseLogAnalyzer analyzer) {
            Analyzer = analyzer as R2RPlusMigrationAnalyzer;
        }
    }
}
