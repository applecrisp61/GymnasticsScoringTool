using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace GymnasticsScoringTool {

    [DataContract]
    public class QualificationParameters : INotifyPropertyChanged {

        [DataMember]
        private string _meetQualfiedFor = "Sea-King 3A (District 2) Championship";
        [DataMember]
        private int _teamQualifiers = 2;
        [DataMember]
        private int _aaQualifiers = 5;
        [DataMember]
        private int _eventQualifiers = 10;
        [DataMember]
        private List<int> _gymnastsToExclude = new List<int>();

        public QualificationParameters() { }
        public QualificationParameters(QualificationParameters qp) {
            _meetQualfiedFor = qp._meetQualfiedFor;
            _teamQualifiers = qp._teamQualifiers;
            _aaQualifiers = qp.aaQualifiers;
            _eventQualifiers = qp._eventQualifiers;
            _gymnastsToExclude = qp._gymnastsToExclude;
        }

        public string meetQualfiedFor
        {
            get { return _meetQualfiedFor; }
            set
            {
                _meetQualfiedFor = value;
                NotifyPropertyChanged();
            }
        }

        public int teamQualifiers
        {
            get { return _teamQualifiers; }
            set
            {
                _teamQualifiers = value;
                NotifyPropertyChanged();
            }
        }

        public int aaQualifiers
        {
            get { return _aaQualifiers; }
            set
            {
                _aaQualifiers = value;
                NotifyPropertyChanged();
            }
        }

        public int eventQualifiers
        {
            get { return _eventQualifiers; }
            set
            {
                _eventQualifiers = value;
                NotifyPropertyChanged();
            }
        }

        public List<int> gymnastsToExclude
        {
            get { return _gymnastsToExclude; }
            set
            {
                _gymnastsToExclude = value;
                NotifyPropertyChanged();
            }
        }

        public static int parseCompetitorNbr(string textCompetitorNbr) {
            int competitorNbr = 0;
            try { competitorNbr = Int32.Parse(textCompetitorNbr); }
            catch (FormatException fe) {
                throw new Exception("Could not parse competitor number: " + textCompetitorNbr, fe);
            }

            return competitorNbr;
        }

        public static List<int> parseCompetitorNbrsToExclude(string textGymnastsToExclude) {
            List<int> competitorsToExclude = new List<int>();

            if (textGymnastsToExclude == "none") { return competitorsToExclude; }

            List<string> string_competitorsToExclude = new List<string>();
            string string_currentExclusion = "";
            string delimiters = ", ";
            int idx = 0;

            while (idx < textGymnastsToExclude.Length) {
                if (delimiters.Contains(textGymnastsToExclude.Substring(idx, 1))) {
                    if (string_currentExclusion.Length > 0) {
                        string_competitorsToExclude.Add(string_currentExclusion);
                        string_currentExclusion = "";
                    }
                }
                else { string_currentExclusion += textGymnastsToExclude.Substring(idx, 1); }
                ++idx;
            }

            if (string_currentExclusion.Length > 0) { string_competitorsToExclude.Add(string_currentExclusion); }

            foreach (string s in string_competitorsToExclude) {
                int competitorExclusion = parseCompetitorNbr(s);
                competitorsToExclude.Add(competitorExclusion);
            }

            return competitorsToExclude;
        }

        public static string displayCompetitorNbrsToExclude(List<int> competitorNbrsToExclude) {
            string displayOutput = "";
            if (competitorNbrsToExclude.Count == 0) { return "none"; }

            int idx = 0;
            foreach (int i in competitorNbrsToExclude) {
                ++idx;
                displayOutput += i.ToString();
                if (idx < competitorNbrsToExclude.Count) {
                    displayOutput += ", ";
                }
            }

            return displayOutput;
        }

        // INTERFACE IMPLEMENTATION
        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "") {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
