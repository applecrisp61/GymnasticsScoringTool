using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

namespace GymnasticsScoringTool {
    [DataContract]
    public class MeetParameters : INotifyPropertyChanged {

        [DataMember]
        private string _meetName;
        [DataMember]
        private string _meetDate;
        [DataMember]
        private string _meetLocation;

        [DataMember]
        private double _maxScore;
        [DataMember]
        private double _minScore;

        [DataMember]
        private int _competitorsPerTeam;
        [DataMember]
        private int _scoresForCompetition;

        [DataMember]
        private bool _useDivisions;

        // PROPERTIES and their associated implementation of INotifyPropertyChanged
        public string meetName
        {
            get { return _meetName; }
            set { _meetName = value;
                NotifyPropertyChanged();
            }
        }

        public string meetDate
        {
            get { return _meetDate; }
            set
            {
                _meetDate = value;
                NotifyPropertyChanged();
            }
        }

        public string meetLocation
        {
            get { return _meetLocation; }
            set
            {
                _meetLocation = value;
                NotifyPropertyChanged();
            }
        }

        public double maxScore
        {
            get { return _maxScore; }
            set
            {
                _maxScore = value;
                NotifyPropertyChanged();
            }
        }

        public double minScore
        {
            get { return _minScore; }
            set
            {
                _minScore = value;
                NotifyPropertyChanged();
            }
        }

        public int competitorsPerTeam
        {
            get { return _competitorsPerTeam; }
            set
            {
                _competitorsPerTeam = value;
                NotifyPropertyChanged();
            }
        }

        public int scoresForCompetition
        {
            get { return _scoresForCompetition; }
            set
            {
                _scoresForCompetition = value;
                NotifyPropertyChanged();
            }
        }

        public bool useDivisions
        {
            get { return _useDivisions; }
            set
            {
                _useDivisions = value;
                NotifyPropertyChanged();
            }
        }

        // CONSTRUCTORS
        public MeetParameters(string name = "Ballard Invitational", 
            string date = "November 12, 2016", 
            string location = "Ballard HS (Seattle, WA)", 
            double max = 10.0, 
            double min = 0.0, 
            int gymnastsPerTeam = 10, 
            int scoresThatCount = 5,
            bool useDiv = false) {
            _meetName = name;
            _meetDate = date;
            _meetLocation = location;
            _maxScore = max;
            _minScore = min;
            _competitorsPerTeam = gymnastsPerTeam;
            _scoresForCompetition = scoresThatCount;
            _useDivisions = useDiv;
        }

        public MeetParameters(MeetParameters mp) :
            this(mp._meetName, mp._meetDate, mp._meetLocation, mp._maxScore, mp._minScore, mp._competitorsPerTeam, mp._scoresForCompetition, mp._useDivisions) 
            { }


        // Method overrides
        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Meet name: " + meetName);
            sb.AppendLine("Date: " + meetDate);
            sb.AppendLine("Location: " + meetLocation);
            sb.AppendLine("Max score: " + maxScore.ToString());
            sb.AppendLine("Min score: " + minScore.ToString());
            sb.AppendLine("Competitors per team: " + competitorsPerTeam.ToString());
            sb.AppendLine("Scores toward team competition: " + scoresForCompetition.ToString());
            sb.AppendLine("Using divisions: " + useDivisions.ToString());

            return sb.ToString();
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
