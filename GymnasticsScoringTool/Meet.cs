using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace GymnasticsScoringTool {
    [DataContract]
    public partial class Meet {

        // DATA MEMBERS and PROPERTIES of the class

        [DataMember]
        private static List<Team> _teams = new List<Team>();

        // Note that ObservableCollection implements INotifyCollectionChanged and INotifyPropertyChanged
        [DataMember]
        private static ObservableCollection<Division> _divisions = new ObservableCollection<Division>();
        [DataMember]
        private static ObservableCollection<Division> _divisionsNotInUse = new ObservableCollection<Division>();

        // Well, this might work, but it blows data encapsulation!! Will want to think about this one more...
        public static ObservableCollection<Division> divisions {
            get {
                return _divisions;
            }
        }

        [DataMember]
        private static List<Gymnast> _gymnasts = new List<Gymnast>();
        private static List<int> _gymnastsToDelete = new List<int>();

        [DataMember]
        private static MeetParameters _meetParameters = new MeetParameters();
        public static MeetParameters meetParameters { get { return _meetParameters; } }

        [DataMember]
        private static QualificationParameters _qualificationParameters = new QualificationParameters();
        public static QualificationParameters qualificationParameters { get { return _qualificationParameters; } }

        public static int TeamCount { get { return _teams.Count; } }
        public static int GymnastCount { get { return _gymnasts.Count; } }
        public static int DivisionCount { get { return _divisions.Count; } }

        private static List<Team> _teamQualifiers;
        private static List<Gymnast> _aaQualifiers;
        private static List<Gymnast> _eventQualifiersVault;
        private static List<Gymnast> _eventQualifiersBars;
        private static List<Gymnast> _eventQualifiersBeam;
        private static List<Gymnast> _eventQualifiersFloor;
        private static List<Gymnast> _qualExcludedGymnasts;

        // CONSTRUCTORS

        static Meet() {
            _divisions.Add(new Division(ProgramConstants.INCLUDE_ALL_DIVISIONS));
            _divisionsNotInUse.Add(new Division(ProgramConstants.DIVISIONS_NOT_IN_USE));
        }

        // METHODS for _teams
        public static bool ContainsTeam(Team t) { return _teams.Contains(t); }
        public static bool ContainsTeam(string teamName) {
            foreach(Team t in _teams) {
                if(t.name == teamName) { return true; }
            }
            return false;
        }

        public static bool ContainsTeam(int nbr) {
            var q = from t in _teams
                    where t.numberBase == nbr
                    select t;

            return q.Any();
        }

        public static int GenerateNextTeamNumber()
        {
            int max = 0;
            foreach (Team t in _teams) {
                if(t.numberBase > max) { max = t.numberBase; }
            }
            return max + ProgramConstants.TEAM_NUMBERING_OFFSET;
        }

        public static Team GetTeamWithNumber(int nbr) {
            return (from t in _teams
                    where t.numberBase == nbr
                    select t).Single(); // should only be one
        }

        public static void AddTeam(Team t) { if(!ContainsTeam(t)) { _teams.Add(t); } }
        public static void RemoveTeam(Team t) {
            if (ContainsTeam(t)) { _teams.Remove(t); }

            var toDelete = from g in _gymnasts
                           where g.team.name == t.name
                           orderby g.competitorNumber descending
                           select g;

            foreach(Gymnast g in toDelete) {
                _gymnasts.Remove(g);
            }
        }

        public static void RemoveGymnastsFromTeam_LastN(Team t, int n) {
            var orderedGymnasts = from g in _gymnasts
                                  where g.team.name == t.name
                                  orderby g.competitorNumber ascending
                                  select g;

            int trimmed = 0;
            int idx = orderedGymnasts.Count() - 1;
            while(trimmed < n) {
                Gymnast toDelete = orderedGymnasts.ElementAt(idx);
                _gymnasts.Remove(toDelete);
                ++trimmed;
                --idx;
            }

        }
        
        public static int TeamSize(int n) {
            var query = from g in _gymnasts
                        where g.team.numberBase == n
                        select g;

            return query.Count();
        }

        public static double? TeamScoreVault(int teamNbr, Division d = null) {
            return (from g in _gymnasts
                    where g.team.numberBase == teamNbr
                    where d == null || g.division.name == d.name || d.name == ProgramConstants.INCLUDE_ALL_DIVISIONS
                    orderby g.IComparableVaultScore descending
                    select g.vaultScore.score).Take(Meet.meetParameters.scoresForCompetition).Sum();
        }

        public static double? TeamScoreBars(int teamNbr, Division d = null) {
            return (from g in _gymnasts
                    where g.team.numberBase == teamNbr
                    where d == null || g.division.name == d.name || d.name == ProgramConstants.INCLUDE_ALL_DIVISIONS
                    orderby g.IComparableBarsScore descending
                    select g.barsScore.score).Take(Meet.meetParameters.scoresForCompetition).Sum();
        }

        public static double? TeamScoreBeam(int teamNbr, Division d = null) {
            return (from g in _gymnasts
                    where g.team.numberBase == teamNbr
                    where d == null || g.division.name == d.name || d.name == ProgramConstants.INCLUDE_ALL_DIVISIONS
                    orderby g.IComparableBeamScore descending
                    select g.beamScore.score).Take(Meet.meetParameters.scoresForCompetition).Sum();
        }

        public static double? TeamScoreFloor(int teamNbr, Division d = null) {
            return (from g in _gymnasts
                    where g.team.numberBase == teamNbr
                    where d == null || g.division.name == d.name || d.name == ProgramConstants.INCLUDE_ALL_DIVISIONS
                    orderby g.IComparableFloorScore descending
                    select g.floorScore.score).Take(Meet.meetParameters.scoresForCompetition).Sum();
        }

        public static double? TeamScoreOverall(int teamNbr, Division division = null) {
            return TeamScoreVault(teamNbr, division) + TeamScoreBars(teamNbr, division) + TeamScoreBeam(teamNbr, division) + TeamScoreFloor(teamNbr, division);
        }

        public static Team GetTeamWithName(string s) {
            foreach (Team t in _teams) {
                if (t.name == s) { return t; }
            }
            return null;
        }
        
        public static List<string> ProvideTeamStringList() {
            List<string> teamStringList = new List<string>();
            foreach(Team t in _teams) {
                teamStringList.Add(t.name);
            }

            return teamStringList;
        }

        public static IEnumerable<Tuple<int, string, string>> ProvideTeamRoster_TupleEnumerable(Team t, Division d = null) {

            var roster = from g in _gymnasts
                         where d == null || g.division.name == d.name || d.name == ProgramConstants.INCLUDE_ALL_DIVISIONS
                         where g.team.name == t.name
                         select new Tuple<int, string, string>(g.competitorNumber, g.firstName, g.lastName);


            return roster;
        }

        public static IEnumerable<IGrouping<string, Gymnast>> GetGymnastListByTeam() {
            return from g in _gymnasts
                        group g by g.team.name;
        }

        public static IEnumerable<Tuple<string, int, string, string>> ProvideGymnastRosters() {
            var roster = from g in _gymnasts
                         orderby g.competitorNumber ascending
                         select new Tuple<string, int, string, string>(g.team.name, g.competitorNumber, g.firstName, g.lastName);
            return roster;
        }

        // METHODS for _divisions
        public static bool ContainsDivision(Division d) { return _divisions.Contains(d); }
        public static bool ContainsDivision(string divisionName) {
            foreach (Division d in _divisions) {
                if (d.name == divisionName) { return true; }
            }
            return false;
        }

        public static int CountOfGymnastsInDivision(Division d) {
            return (from g in _gymnasts
                    where g.division.name == d.name
                    select 1).Count();
        }

        public static void AddDivision(Division d) { if (!ContainsDivision(d)) { _divisions.Add(d); } }
        public static void RemoveDivision(Division d) { if (ContainsDivision(d)) { _divisions.Remove(d); } }
        public static Division GetDivisionWithName(string divisionName) {
            foreach (Division d in _divisions) {
                if (d.name == divisionName) { return d; }
            }

            if((divisionName=="")|| (divisionName==null)) { return null; }
            throw new Exception_DivisionNotImplemented(divisionName);
        }

        public static void ResetDivisions(List<Division> listForReset) {
            _divisions.Clear();
            foreach(Division d in listForReset) {
                _divisions.Add(new Division(d));
            }
        }

        public static void ProgrammaticallyAdjustDivisions() {
            _divisions.Clear();
            Division newBase = new Division(ProgramConstants.INCLUDE_ALL_DIVISIONS);
            _divisions.Add(newBase);

            foreach(Gymnast g in _gymnasts) {
                g.division = newBase;
            }

            
        }

        // METHODS for _gymnasts
        public static bool ContainsGymnast(Gymnast g) { return _gymnasts.Contains(g); }
        public static bool ContainsGymnast(int nbr) {
            foreach (Gymnast g in _gymnasts) {
                if (g.competitorNumber == nbr) { return true; }
            }
            return false;
        }
        public static bool ContainsGymnast(string fName, string lName, string teamName) {
            foreach (Gymnast g in _gymnasts) {
                if ((g.firstName== fName) && (g.lastName == lName) && (g.team.name == teamName)) { return true; }
            }
            return false;
        }

        public static int CountOfGymnastsToRankByDivision(string eventString, Division d = null) {
            var gymnastsInDivision = (from g in _gymnasts
                                      where (d == null || g.division.name == d.name || d.name == ProgramConstants.INCLUDE_ALL_DIVISIONS)
                                      select g);

            switch (eventString.ToUpper()) {
            case "VAULT":
                return (from g in gymnastsInDivision where g.vaultScore.score.HasValue select g).Count();
            case "BARS":
                return (from g in gymnastsInDivision where g.barsScore.score.HasValue select g).Count();
            case "BEAM":
                return (from g in gymnastsInDivision where g.beamScore.score.HasValue select g).Count();
            case "FLOOR":
                return (from g in gymnastsInDivision where g.floorScore.score.HasValue select g).Count();
            case "OVERALL":
            case "ALL-AROUND":
            case "ALL AROUND":
            case "INDIVIDUAL ALL AROUND":
            case "INDIVIDUAL ALL-AROUND":
            case "INDIVIDUAL OVERALL":
                return (from g in gymnastsInDivision
                        where g.IncludeInAllAroundResults
                        select g).Count();
            default:
                throw new Exception_UnkownEvent(eventString);
            }
        }

        public static int GenerateNextGymnastNumberOnTeam(Team team) {
            if (team.rosterSize == 0) {
                return 0;
            }

            int max = 0;

            var assignedNumbers = from g in _gymnasts
                              where g.team.numberBase == team.numberBase
                              select g.competitorNumber - g.team.numberBase;

            foreach (int i in assignedNumbers) {
                if (i > max) { max = i; }
            }
            return max + 1;
        }

        public static Gymnast GetGymnastByNumber(int nbr) {
            if (ContainsGymnast(nbr)) {
                return (from g in _gymnasts
                        where g.competitorNumber == nbr
                        select g).Single();
            }
            else return null;
        }

        public static void AddGymnast(Gymnast g) { if (!ContainsGymnast(g)) { _gymnasts.Add(g); } }
        public static void RemoveGymnast(Gymnast g) { if (ContainsGymnast(g)) { _gymnasts.Remove(g); } }
        public static void RemoveGymnast(int nbr) { RemoveGymnast(GetGymnast(nbr)); }
        private static Gymnast GetGymnast(int competitorNbr) {
            Gymnast query = (from g in _gymnasts
                        where g.competitorNumber == competitorNbr
                        select g).Single(); // Note: Uniqueness of competitorNbr is enforced within gymnast creation

            return query;
        }

        public static void MarkGymnastForDeletion(int nbr) {
            if(ContainsGymnast(nbr) && !_gymnastsToDelete.Contains(nbr)) {
                _gymnastsToDelete.Add(nbr);
            } 
        }

        public static void RemoveGymnastsMarkedForDeletion() {
            foreach (int i in _gymnastsToDelete) {
                RemoveGymnast(i);
            }

            _gymnastsToDelete.Clear();
        }

        public static void ResetGymnastsMarkedForDeletion() { _gymnastsToDelete.Clear(); }

        public static void UpdateGymnastDivision(int competitorNbr, Division d) {
            GetGymnast(competitorNbr).division = d;
        }

        public static void EditGymnast(int competitorNbr, string fName, string lName,
            string vaultString, string barsString, string beamString, string floorString, string divString) {

            double? vaultDoub, barsDoub, beamDoub, floorDoub;
            try { vaultDoub = double.Parse(vaultString); } catch { vaultDoub = null; }
            try { barsDoub = double.Parse(barsString); } catch { barsDoub = null; }
            try { beamDoub = double.Parse(beamString); } catch { beamDoub = null; }
            try { floorDoub = double.Parse(floorString); } catch { floorDoub = null; }
            Division division = GetDivisionWithName(divString);

            Gymnast g = GetGymnast(competitorNbr);
            g.firstName = fName;
            g.lastName = lName;
            // Note that edits to gymnast are performed within the context of the team;
            // therefore, to change team, must remove gymnast from team and recreate gymnast
            // again on the new team (should be rare)
            g.vaultScore = new EventScore(vaultDoub);
            g.barsScore = new EventScore(barsDoub);
            g.beamScore = new EventScore(beamDoub);
            g.floorScore = new EventScore(floorDoub);
            g.division = division;
        }

        public static Tuple<EventScore, EventScore, EventScore, EventScore> GetGymnastEventScoresTuple(int nbr) {
            Gymnast g = GetGymnast(nbr);

            // copy the scores to new event scores
            EventScore vault = new EventScore(g.vaultScore.score);
            EventScore bars = new EventScore(g.barsScore.score);
            EventScore beam = new EventScore(g.beamScore.score);
            EventScore floor = new EventScore(g.floorScore.score);

            return new Tuple<EventScore, EventScore, EventScore, EventScore>(vault, bars, beam, floor);
        }


        public static bool GymnastScoreUsedByTeamForEvent(string eventString, int competitorNbr, Division d = null) {

            Gymnast gymnast = GetGymnast(competitorNbr);

            IEnumerable<Gymnast> rankedGymnasts;

            switch (eventString.ToUpper()) {
            case "VAULT":
                rankedGymnasts = from g in _gymnasts
                                 where g.team.numberBase == gymnast.team.numberBase
                                 where (d == null || d.name == "" || g.division.name == d.name)
                                 orderby g.IComparableVaultScore descending
                                 select g;
                break;
            case "BARS":
                rankedGymnasts = from g in _gymnasts
                                 where g.team.numberBase == gymnast.team.numberBase
                                 where (d == null || d.name == "" || g.division.name == d.name)
                                 orderby g.IComparableBarsScore descending
                                 select g;
                break;
            case "BEAM":
                rankedGymnasts = from g in _gymnasts
                                 where g.team.numberBase == gymnast.team.numberBase
                                 where (d == null || d.name == "" || g.division.name == d.name)
                                 orderby g.IComparableBeamScore descending
                                 select g;
                break;
            case "FLOOR":
                rankedGymnasts = from g in _gymnasts
                                 where g.team.numberBase == gymnast.team.numberBase
                                 where (d == null || d.name == "" || g.division.name == d.name)
                                 orderby g.IComparableFloorScore descending
                                 select g;
                break;
            default:
                throw new Exception_UnkownEvent(eventString);
            }

            int idx = 0;
            foreach(Gymnast g in rankedGymnasts) {
                if(g.competitorNumber == competitorNbr) { return true; }
                
                ++idx;
                if(idx >= _meetParameters.scoresForCompetition) { return false; }
                
            }

            return false;

        }

        /*
        public async static Task sendTeamsForSerialization() {
            string dateString = DateTime.Today.Year.ToString() + (DateTime.Today.Month < 10 ? "0" : "") + DateTime.Today.Month.ToString()
                + (DateTime.Today.Day < 10 ? "0" : "") + DateTime.Today.Day.ToString();

            StringBuilder filename = new StringBuilder();
            foreach (Team t in _teams) {
                filename.Clear();
                filename.Append(t.name + " " + dateString + ".team");

                var gymnastsToSerialize = from g in _gymnasts
                                          where g.team.name == t.name
                                          select g;

                await FileManagement.Serialize<IEnumerable<Gymnast>>(gymnastsToSerialize, "Team File", ".team", filename.ToString(), @"Serialization\TeamFiles");

            }
        }
        */

        // SERIALIZATION and RECOVERY 

        // private static string _saveFileFolderAdj = "";

        public async static Task sendForSerialization(string filename = "", string folderAdj = "") {
            // _saveFileFolderAdj = folderAdj;

            MeetSerializeAndRestoreClass msarc = new MeetSerializeAndRestoreClass(_divisions, _teams, _gymnasts, _meetParameters, _qualificationParameters);

            await FileManagement.Serialize<MeetSerializeAndRestoreClass>(msarc, "Meet File", ".meet", filename, folderAdj);
        }

        public async static Task restoreFromSerialization(string filename = "", string folderAdj = "") {

            var msarc = await FileManagement.Recover<MeetSerializeAndRestoreClass>(".meet", filename, folderAdj);

            Meet._divisions = new ObservableCollection<Division>();
            Meet._teams = new List<Team>();
            Meet._gymnasts = new List<Gymnast>();

            Meet._divisions = msarc.provideDivisionListing();
            Meet._teams = msarc.provideTeamListing();
            Meet._gymnasts = msarc.provideGymnastListing();

            // set each of these individually so that the INotifyPropertyChanged interface can work its magic with data bindings
            Meet._meetParameters.meetName = msarc.provideMeetParameters().meetName;
            Meet._meetParameters.meetLocation = msarc.provideMeetParameters().meetLocation;
            Meet._meetParameters.meetDate = msarc.provideMeetParameters().meetDate;
            Meet._meetParameters.minScore = msarc.provideMeetParameters().minScore;
            Meet._meetParameters.maxScore = msarc.provideMeetParameters().maxScore;
            Meet._meetParameters.competitorsPerTeam = msarc.provideMeetParameters().competitorsPerTeam;
            Meet._meetParameters.scoresForCompetition = msarc.provideMeetParameters().scoresForCompetition;
            Meet._meetParameters.useDivisions = msarc.provideMeetParameters().useDivisions;

            Meet._qualificationParameters.meetQualfiedFor = msarc.provideQualificationParameters().meetQualfiedFor;
            Meet._qualificationParameters.teamQualifiers = msarc.provideQualificationParameters().teamQualifiers;
            Meet._qualificationParameters.aaQualifiers = msarc.provideQualificationParameters().aaQualifiers;
            Meet._qualificationParameters.eventQualifiers = msarc.provideQualificationParameters().eventQualifiers;
            Meet._qualificationParameters.gymnastsToExclude = msarc.provideQualificationParameters().gymnastsToExclude;
        }



    }
}
