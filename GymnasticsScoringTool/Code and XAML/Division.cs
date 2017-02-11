using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;


namespace GymnasticsScoringTool_01 {
    [DataContract]
    public class Division : IEquatable<Division> {

        [DataMember]
        private string _name;
        public string name
        {
            get { return _name; }
        }

        // Constructor
        public Division(string divisionName) {
            if (Meet.ContainsDivision(divisionName)) {
                throw new Exception_DuplicateDivisionCreationAttempt(divisionName);
            }

            _name = divisionName;
        }

        // Copy Constructor
        public Division(Division d)
        : this(d.name) { }
    
        // Overrides
        public override string ToString() {
            return name;
        }

        public override bool Equals(object obj) {
            if(object.ReferenceEquals(obj, null)) { return false; }
            if(object.ReferenceEquals(this, obj)) { return true; }
            if(this.GetType() != obj.GetType()) { return false; }
            return Equals(obj as Division);
        }

        public override int GetHashCode() {
            return _name.GetHashCode();
        }

        // Interface implementations
        // IEquatable<Division>
        public bool Equals(Division other) {
            return this.name == other.name;
        }
    }
}
