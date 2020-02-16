﻿using System.Linq;

namespace LogAnalyzer.Analyzers.Errors.Database {
    public class OutOfConnectionsErrorAnalyzer<T> : ErrorSummarizer<T> {
        public override string NoErrorFoundMessage => "No all-connections-in-use errors found";

        public OutOfConnectionsErrorAnalyzer() {
            SubstringsToMatch.Add("all pooled connections were in use");
        }

        public override string AnalysesToString() {
            if (LineNumbers.Any()) {
                return $"All-connections-in-use errors found: {LineNumbers.Count}, starting at line {LineNumbers.First()}: {ErrorMessage}";
            }

            return NoErrorFoundMessage;
        }

    }
}
