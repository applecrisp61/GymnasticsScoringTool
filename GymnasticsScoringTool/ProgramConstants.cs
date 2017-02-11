using System;

namespace GymnasticsScoringTool {
    public static class ProgramConstants {

        public static readonly string NAME_OF_APP = "Gymnastics Scoring Tool";

        public static readonly string AUTOSAVE_FOLDER_ADJ = "Autosave";
        public static readonly string AUTOSAVE_FILE_NAME = "SafetyRestore.meet";

        public static readonly string EXCEPTION_LOG_FOLDER_ADJ = "ExceptionLog";
        public static readonly string EXCEPTION_LOG_FILE_NAME = "ExceptionLog.txt";

        public static readonly double NULL_ICOMPARISON_SCORE = -1;
        public static readonly string NULL_SCORE_STRING = "-.-";

        public static readonly string KEYPAD_ENTRY = "Enter by keypad";

        public static readonly int TEAM_NUMBERING_OFFSET = 100;

        public static readonly string NO_EXCEPTION_MSG = "No exception / no message";

        public static readonly string TEAM_NOT_SET_TEXT = "Must specify a new TEAM first";

        public static readonly string INCLUDE_ALL_DIVISIONS = "None set";
        public static readonly string NO_DIVISION_SET = "None set";
        public static readonly string DIVISIONS_NOT_IN_USE = "Not in use";

        public static readonly string TITLE_EXCEPTION_DURING_DIVISION_CONFIG = "EXCEPTION: During Division Configuration";
        public static readonly string DIVISION_ALREADY_EXISTS = "Division already exists"
            + Environment.NewLine + Environment.NewLine + "No changes to divisions made";
        public static readonly string DIVISION_NOT_EMPTY = "Gymnasts are still asssigned to this division. "
            + "Division must be empty before it can be deleted. Please reassign gymnasts currently in division and try again."
            + Environment.NewLine + Environment.NewLine + "No changes to divisions made";

        public static readonly string DEFAULT_NAME = "Default";

        // FORMATTING of score for presentation 
        public static readonly int DIGITS_BEFORE_DOT_TEAM = 3;
        public static readonly int DIGITS_BEFORE_DOT = 2;
        public static readonly int DIGITS_AFTER_DOT = 3;

        public static readonly string POORLY_FORMATTED_SCORE = "Event score not properly formatted; must be a decimal number (or -.- for did not participate)."
            + Environment.NewLine + Environment.NewLine + "Score will NOT be updated.";
        public static readonly string SCORE_OUTSIDE_VALID_RANGE = "Event score outside valid range of " + Meet.meetParameters.minScore.ToString() 
            + " to " + Meet.meetParameters.maxScore.ToString() + "."
            + Environment.NewLine + Environment.NewLine + "Score will NOT be updated.";
        public static readonly string POORLY_FORMATTED_COMPETITOR_NBR = "Poorly formatted competitor number - cannot be converted into an integer.\n\nCannot be used for score update";
        public static readonly string INVALID_COMPETITOR_NBR_MSG = "Invalid competitor number - no registered gymnast has that number.\n\nCannot be used for score update";
        public static readonly string KEYPAD_GYMNAST_SCORE_ALREADY_ENTERED = "Invalid competitor number - gymnast already has score entered for this event\n\nPlease edit elsewhere";
        public static readonly string TITLE_INVALID_SCORE = "INVALID SCORE" + Environment.NewLine;
        public static readonly string TITLE_INVALID_COMPETITOR_NBR = "INVALID COMPETITOR NUMBER" + Environment.NewLine;
        public static readonly int INVALID_COMPETITOR_NBR = -1;
        public static readonly double INVALID_EVENT_SCORE = -1;
        public static readonly double EPSILON = 0.000001;

        public static readonly string TITLE_TEAM_NOT_YET_CREATED = "TEAM NOT YET CREATED";

        public static readonly string TEAM_NOT_YET_CREATED_MSG =
            "No team has yet been created; must first create team before you can add gymnast";

        public static readonly int MIN_INTEGER_PARAM = 0;
        public static readonly int MAX_INTEGER_PARAM = Int16.MaxValue; // 32,767... 2^15... should be enough
        public static readonly double MIN_DECIMAL_PARAM = 0;
        public static readonly double MAX_DECIMAL_PARAM = (double)Int16.MaxValue;

        public static readonly string POORLY_FORMATTED_INTEGER_PARAM = "Integer parameter not properly formatted; must be a whole number greater than or equal to "
            + MIN_INTEGER_PARAM.ToString() + " and less than or equal to " + MAX_INTEGER_PARAM.ToString() + "." 
            + Environment.NewLine + Environment.NewLine + "Parameter will NOT be updated.";
        public static readonly string INTEGER_PARAM_OUTSIDE_VALID_RANGE = "Integer parameter outside valid range of " + MIN_INTEGER_PARAM.ToString()
            + " to " + MAX_INTEGER_PARAM.ToString() + "."
            + Environment.NewLine + Environment.NewLine + "Parameter will NOT be updated.";
        public static readonly string TITLE_INVALID_INTEGER_PARAM = "INVALID INTEGER PARAMETER" + Environment.NewLine;

        public static readonly string POORLY_FORMATTED_DECIMAL_PARAM = "Decimal parameter not properly formatted; must be a decimal number greater than or equal to "
            + MIN_DECIMAL_PARAM.ToString() + " and less than or equal to " + MAX_DECIMAL_PARAM.ToString() + "."
            + Environment.NewLine + Environment.NewLine + "Parameter will NOT be updated.";
        public static readonly string DECIMAL_PARAM_OUTSIDE_VALID_RANGE = "Decimal parameter outside valid range of " + MIN_DECIMAL_PARAM.ToString()
            + " to " + MAX_DECIMAL_PARAM.ToString() + "."
            + Environment.NewLine + Environment.NewLine + "Parameter will NOT be updated.";
        public static readonly string TITLE_INVALID_DECIMAL_PARAM = "INVALID DECIMAL PARAMETER" + Environment.NewLine;

        // PRINT and DISPLAY SIZING elements
        public static readonly int STANDARD_OUTPUT_WIDTH = 528; // (5.5 logical inches @ 96 logical pixels per logical inch)
        public static readonly int STANDARD_OUTPUT_ROW_HEIGHT = 18;

    }
}
