using System;
using System.Runtime.Serialization;

namespace GymnasticsScoringTool {
    [DataContract]
    public class Team : IEquatable<Team> {

        // DATA MEMBERS and PROPERTIES
        [DataMember]
        private string _name;
        public string name
        {
            get { return _name; }
        }

        [DataMember]
        private int _number;
        public int number
        {
            get { return _number; }
        }

        public int rosterSize
        {
            get
            {
                return Meet.TeamSize(this.number);
            }
        }

        public double? teamScoreVault
        {
            get
            {
                return Meet.TeamScoreVault(this.number);
            }
        }

        public double? teamScoreBars
        {
            get
            {
                return Meet.TeamScoreBars(this.number);
            }
        }

        public double? teamScoreBeam
        {
            get
            {
                return Meet.TeamScoreBeam(this.number);
            }
        }

        public double? teamScoreFloor
        {
            get
            {
                return Meet.TeamScoreFloor(this.number);
            }
        }

        public double? teamScoreOverall
        {
            get
            {
                return Meet.TeamScoreOverall(this.number);
            }
        }

        public double? teamScore(string eventString, Division d) {

            switch (eventString.ToUpper()) {
            case "VAULT":
                return Meet.TeamScoreVault(this.number, d);
            case "BARS":
                return Meet.TeamScoreBars(this.number, d);
            case "BEAM":
                return Meet.TeamScoreBeam(this.number, d);
            case "FLOOR":
                return Meet.TeamScoreFloor(this.number, d);
            case "OVERALL":
            case "ALL-AROUND":
            case "ALL AROUND":
            case "INDIVIDUAL ALL AROUND":
            case "INDIVIDUAL ALL-AROUND":
            case "INDIVIDUAL OVERALL":
                return Meet.TeamScoreOverall(this.number, d);
            default:
                throw new Exception_UnkownEvent(eventString);
            }
        }

        // CONSTRUCTORS
        public Team(string teamName) :
            this(teamName, Meet.GenerateNextTeamNumber())  
            {}

        private Team(string teamName, int teamNumber) 
        {
            if (Meet.ContainsTeam(teamName)) {
                throw new Exception_DuplicateTeamCreationAttempt(teamName);
            }

            _name = teamName;
            _number = teamNumber;
        }

        public Team(Team t) :
            this(t.name, t.number)
            {}

        // Overrides
        public override string ToString() {
            return name;
        }

        public override bool Equals(object obj) {
            if (object.ReferenceEquals(obj, null)) { return false; }
            if (object.ReferenceEquals(this, obj)) { return true; }
            if (this.GetType() != obj.GetType()) { return false; }
            return Equals(obj as Division);
        }

        public override int GetHashCode() {
            return _name.GetHashCode();
        }

        // Interface implementations
        // IEquatable<Division>
        public bool Equals(Team other) {
            return this.name == other.name;
        }
    }
}
