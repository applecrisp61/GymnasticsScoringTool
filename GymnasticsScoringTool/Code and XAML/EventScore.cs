using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;


namespace GymnasticsScoringTool_01 {
    [DataContract]
    public class EventScore {

        [DataMember] private double? _score;
        public double? score
        {
            get { return _score; }
            set {
                if (!value.HasValue) { _score = value; }
                else if ((value.Value >= Meet.meetParameters.minScore) && (value.Value <= Meet.meetParameters.maxScore)) { _score = value; }
                else { throw new Exception_ScoreOutOfValidRange(value.Value, Meet.meetParameters.minScore, Meet.meetParameters.maxScore); }
            }
        }
        public double iComparerScore { get { return score.HasValue ? score.Value : ProgramConstants.NULL_ICOMPARISON_SCORE; } }
        public string displayScore
        {
            get { return ToString(); }
            set
            {
                if (IsValid(value)) {
                    // duplication in validation and string adjustment here (DCL 2016/11/10)
                    if (value == ProgramConstants.NULL_SCORE_STRING) { score = null; }
                    else {
                        double? input = -1;
                        try { input = Double.Parse(value); }
                        catch { } // do nothing... leave the score as it is
                        try { score = input; }
                        catch { } // again, do nothing... leave the score as it is
                    }
                }
            }
        }

        public double? previousScore { get; set; }

        // Constructors
        public EventScore(double? inputScore = null) {
            score = inputScore;
        }

        // Static validation methods
        public static bool IsValid(string inputString) {
            string adjStr = AdjustInputString(inputString);
            return !IsPoorlyFormatted(adjStr) && !IsOutsideValidRange(adjStr);
        }

        public static bool IsPoorlyFormatted(string inputString) {
            if (inputString == ProgramConstants.NULL_SCORE_STRING) { return false; }
            double inputValue = -1;
            if (!Double.TryParse(inputString, out inputValue)) { return true; }
            return false;
        }

        public static bool IsOutsideValidRange(string inputString) {
            if (inputString == ProgramConstants.NULL_SCORE_STRING) { return false; }
            double inputValue = -1;
            if (Double.TryParse(inputString, out inputValue)) {
                if (inputValue < Meet.meetParameters.minScore) { return true; }
                if (inputValue > Meet.meetParameters.maxScore) { return true; }
            }
            return false;
        }

        public static string AdjustInputString(string inputString) {
            string adjStr = inputString;
            if (adjStr == ProgramConstants.NULL_SCORE_STRING) { return adjStr; }
                if (Meet.meetParameters.maxScore > 10) { return adjStr; } // Disable quick entry if scores > 10 are usable
            if ((adjStr.Length > 1) && (!adjStr.Contains(".")) && ((adjStr != "10") || (adjStr != "100") || (adjStr != "1000"))) {
                adjStr = adjStr.Substring(0, 1) + "." + adjStr.Substring(1);
            }
            else if((adjStr == "10") || (adjStr == "100") || (adjStr == "1000")) {
                adjStr = "10.0";
            }
            return adjStr;
        }

        // Operators

        // Note: null is being used to represent the concept of "DID NOT COMPETE"
        // This creates some interpretations that are a bit different than the traditional usage of null

        // When used in addition, DID NOT COMPETE should be treated the same as 0.0
        // When used in comparison, DID NOT COMPETE should be treated as less than 0.0
        // DID NOT COMPETE should be treated as equal to DID NOT COMPETE

        // Overloaded operator +
        public static double? operator +(EventScore es1, EventScore es2) {
            double? result = new double?();
            if ((!es1.score.HasValue) && (!es2.score.HasValue)) { return result; }
            else if (!es1.score.HasValue) { return es2.score; }
            else if (!es2.score.HasValue) { return es1.score; }
            else { return (es1.score + es2.score); }
        }

        public static double? operator +(EventScore es1, double? d2) {
            double? result = new double?();
            if ((!es1.score.HasValue) && (!d2.HasValue)) { return result; }
            else if (!es1.score.HasValue) { return d2.Value; }
            else if (!d2.HasValue) { return es1.score; }
            else { return (es1.score + d2); }
        }

        public static double? operator +(double? d1, EventScore es2) {
            double? result = new double?();
            if ((!d1.HasValue) && (!es2.score.HasValue)) { return result; }
            else if (!d1.HasValue) { return es2.score; }
            else if (!es2.score.HasValue) { return d1.Value; }
            else { return (d1 + es2.score); }
        }


        // Overloaded operators >, >=, <, <=, ==
        public static bool operator >(EventScore es1, EventScore es2) {
            if (!es1.score.HasValue) return false;
            else if (!es2.score.HasValue) return true;
            return es1.score > es2.score;
        }

        public static bool operator >=(EventScore es1, EventScore es2) {
            if (!es1.score.HasValue && !es2.score.HasValue) { return true; }
            if (!es1.score.HasValue) { return false; }
            if (!es2.score.HasValue) { return true; }
            return es1.score >= es2.score;
        }

        public static bool operator <(EventScore es1, EventScore es2) {
            if (!es1.score.HasValue) return true;
            else if (!es2.score.HasValue) return false;
            return es1.score < es2.score;
        }

        public static bool operator <=(EventScore es1, EventScore es2) {
            if (!es1.score.HasValue && !es2.score.HasValue) { return true; }
            if (!es1.score.HasValue) { return true; }
            if (!es2.score.HasValue) { return false; }
            return es1.score <= es2.score;
        }

        public static bool operator ==(EventScore es1, EventScore es2) {
            if (!es1.score.HasValue && !es2.score.HasValue) { return true; }
            if (!es1.score.HasValue) { return false; }
            if (!es2.score.HasValue) { return false; }
            return es1.score == es2.score;
        }

        public static bool operator !=(EventScore es1, EventScore es2) {
            if (!es1.score.HasValue && !es2.score.HasValue) { return false; }
            if (!es1.score.HasValue) { return true; }
            if (!es2.score.HasValue) { return true; }
            return es1.score != es2.score;
        }

        // Overload method GetHashCode()
        public override int GetHashCode() {
            return base.GetHashCode();
        }

        // Overload method Equals()
        public override bool Equals(object obj) {
            return base.Equals(obj);
        }

        // Overload method ToString()
        public override string ToString() {
            if (!score.HasValue) { return ProgramConstants.NULL_SCORE_STRING; }
            if (score == null) { return ProgramConstants.NULL_SCORE_STRING; }

            string temp = score.ToString();
            if (!temp.Contains(".")) {
                temp += ".0";
            }
            return temp;
        }


    }
}
