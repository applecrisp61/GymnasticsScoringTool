using System;

namespace GymnasticsScoringTool {
    public class Exception_ScoreOutOfValidRange : Exception {

        private double _score;
        private double _min;
        private double _max;

        public Exception_ScoreOutOfValidRange(double score, double min, double max) {
            _score = score;
            _min = min;
            _max = max;
        }

        public override string Message
        {
            get
            {
                return "EXCEPTION: score of " + _score.ToString() + " not within valid range of " + _min.ToString() + " to " + _max.ToString();
            }
        }
    }

    public class Exception_DuplicateTeamCreationAttempt : Exception {

        string _name;

        public Exception_DuplicateTeamCreationAttempt(string name) {
            _name = name;
        }

        public override string Message
        {
            get
            {
                return "EXCEPTION: team with name " + _name + " already exists";
            }
        }
    }

    public class Exception_DuplicateDivisionCreationAttempt : Exception {

        string _name;

        public Exception_DuplicateDivisionCreationAttempt(string name) {
            _name = name;
        }

        public override string Message
        {
            get
            {
                return "EXCEPTION: division with name " + _name + " already exists";
            }
        }
    }

    public class Exception_TeamNotRegistered : Exception {

        string _name;

        public Exception_TeamNotRegistered(string name) {
            _name = name;
        }

        public override string Message
        {
            get
            {
                return "EXCEPTION: team with name " + _name + " is not currently registered for this meet";
            }
        }
    }

    public class Exception_DivisionNotImplemented : Exception {

        string _name;

        public Exception_DivisionNotImplemented(string name) {
            _name = name;
        }

        public override string Message
        {
            get
            {
                return "EXCEPTION: division with name " + _name + " has not yet been implemented for this meet";
            }
        }
    }

    public class Exception_DuplicateGymnastCreationAttempt : Exception {

        string _firstName;
        string _lastName;
        string _teamName;

        public Exception_DuplicateGymnastCreationAttempt(string fName, string lName, string tName) {
            _firstName = fName;
            _lastName = lName;
            _teamName = tName;
        }

        public override string Message
        {
            get
            {
                return "EXCEPTION: gymnast named " + _firstName + " " + _lastName + " already exists on team " + _teamName;
            }
        }
    }

    public class Exception_UnkownEvent : Exception {

        string _name;

        public Exception_UnkownEvent(string name) {
            _name = name;
        }

        public override string Message
        {
            get
            {
                return "EXCEPTION: unknown event " + _name;
            }
        }
    }

    public class Exception_UnexpectedDataTypeEncountered : Exception {
        Type _expectedType;
        Type _encounteredType;

        public Exception_UnexpectedDataTypeEncountered(Type expected, Type encountered) {
            _expectedType = expected;
            _encounteredType = encountered;
        }

        public override string Message
        {
            get
            {
                return "EXCEPTION: expected data type " + _expectedType.Name + " but enountered data type " + _encounteredType.Name;
            }
        }
    }

    public class Exception_CouldNotFindUIElement : Exception {
        string _uiElementName;
        Type _expectedType;
        Type _callingType;
        string _visualTreeString;

        public Exception_CouldNotFindUIElement(string uiElementName, Type expectedType, Type callingType, string visualTreeString = null) {
            _uiElementName = uiElementName;
            _expectedType = expectedType;
            _callingType = callingType;
            _visualTreeString = visualTreeString;
        }

        public override string Message
        {
            get
            {
                if(_visualTreeString==null) {
                    return "EXCEPTION: Could not find UI Element named " + _uiElementName + " of expected type " + _expectedType.Name + " in call from type " + _callingType.Name;
                }
                else {
                    string returnString = "EXCEPTION: Could not find UI Element named " + _uiElementName + " of expected type " + _expectedType.Name + " in call from type " + _callingType.Name;
                    returnString += Environment.NewLine + _visualTreeString;

                    return returnString;
                }
                
            }
        }
    }
}
