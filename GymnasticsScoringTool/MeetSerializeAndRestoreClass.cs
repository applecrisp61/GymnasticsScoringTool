using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace GymnasticsScoringTool {
    [DataContract]
    class MeetSerializeAndRestoreClass {

        [DataMember]
        private ObservableCollection<Division> _divisions;

        [DataMember]
        private List<Team> _teams;

        [DataMember]
        private List<Gymnast> _gymnasts;

        [DataMember]
        private MeetParameters _meetParameters;

        [DataMember]
        private QualificationParameters _qualificationParameters;


        public MeetSerializeAndRestoreClass(ObservableCollection<Division> divisions, List<Team> teams, List<Gymnast> gymnasts, 
            MeetParameters meetParameters, QualificationParameters qualificationParameters) {
            _divisions = divisions;
            _teams = teams;
            _gymnasts = gymnasts;
            _meetParameters = meetParameters;
            _qualificationParameters = qualificationParameters;
        }

        // Methods
        public ObservableCollection<Division> provideDivisionListing() {
            var output = new ObservableCollection<Division>();
            foreach(Division d in _divisions) { output.Add(new Division(d)); }
            return output;
        }

        public List<Team> provideTeamListing() {
            var output = new List<Team>();
            foreach (Team t in _teams) { output.Add(new Team(t)); }
            return output;
        }

        public List<Gymnast> provideGymnastListing() {
            var output = new List<Gymnast>();
            foreach (Gymnast g in _gymnasts) { output.Add(new Gymnast(g)); }
            return output;
        }

        public MeetParameters provideMeetParameters() {
            return new MeetParameters(_meetParameters);
        }

        public QualificationParameters provideQualificationParameters() {
            return new QualificationParameters(_qualificationParameters);
        }
    }
}
