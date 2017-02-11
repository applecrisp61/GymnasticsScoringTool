using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using GymnasticsScoringTool_01;

namespace UnitTestProject {
    [TestClass]
    public class UnitTest1 {

        #region Declarations.
        private static Gymnast stephanieGundel;
        private static Gymnast lynneaCommon;
        private static Gymnast annaUngar;
        private static Gymnast spencerLetzler;
        private static Gymnast margaretThatcher;
        private static Gymnast soniaGandhi;

        private static Gymnast maryLouRetton;
        private static Gymnast shannonMiller;
        private static Gymnast dominiqueDawes;
        private static Gymnast nastyaLiukin;
        private static Gymnast gabbyDouglas;
        private static Gymnast alyRaisman;
        private static Gymnast simoneBiles;

        private static Gymnast bettyFlipsalot;
        private static Gymnast tinaTwister;
        private static Gymnast sandraSpinCycleOfDoom;
        private static Gymnast jumpyWombats;
        private static Gymnast hildaOopssplat;
        private static Gymnast flexiRollerball;
        private static Gymnast eferEffort;

        private static Team dreamTeam;
        private static Team olympicTypes;
        private static Team imaginaryWonders;

        private static string testMeetFirstRoster;
        private static string testMeetSecondRoster;
        private static string testMeetTestDivisionRoster;
        private static string testMeetBarsStandings;
        private static string testMeetBeamStandings;
        private static string testMeetFloorStandings;
        private static string testMeetVaultStandings;
        private static string testMeetOverallStandings;

        private static string testMeetUpdatedRoster;
        private static string testMeetUpdatedTestDivisionRoster;
        private static string testMeetUpdatedBarsStandings;
        private static string testMeetUpdatedBeamStandings;
        private static string testMeetUpdatedFloorStandings;
        private static string testMeetUpdatedVaultStandings;
        private static string testMeetUpdatedOverallStandings;
        private static string testMeetUpdatedBarsStandings_TestDivision;
        private static string testMeetUpdatedBeamStandings_TestDivision;
        private static string testMeetUpdatedFloorStandings_TestDivision;
        private static string testMeetUpdatedVaultStandings_TestDivision;
        private static string testMeetUpdatedOverallStandings_TestDivision;

        #endregion Declarations.

        [ClassInitialize]
        public static void UnitTest_Initialize(TestContext testContext) {

            // For automatic assignment of numbers to work, items must be added to Meet as they are created rather than generated in batch then added

            dreamTeam = new Team("Dream Team");
            Meet.AddTeam(dreamTeam);

            olympicTypes = new Team("Olympic Types");
            Meet.AddTeam(olympicTypes);

            imaginaryWonders = new Team("Imaginary Wonders");
            Meet.AddTeam(imaginaryWonders);

            stephanieGundel = new Gymnast("Stephanie", "Gundel", dreamTeam, "Open");
            stephanieGundel.barsScore = new EventScore(9.85);
            stephanieGundel.beamScore = new EventScore(9.7);
            stephanieGundel.floorScore = new EventScore(9.65);
            stephanieGundel.vaultScore = new EventScore(9.95);
            Meet.AddGymnast(stephanieGundel);

            lynneaCommon = new Gymnast("Lynnea", "Common", dreamTeam, "Open");
            lynneaCommon.barsScore = new EventScore(9.8);
            lynneaCommon.beamScore = new EventScore(9.75);
            lynneaCommon.floorScore = new EventScore(9.7);
            lynneaCommon.vaultScore = new EventScore(9.55);
            Meet.AddGymnast(lynneaCommon);

            annaUngar = new Gymnast("Anna", "Ungar", dreamTeam, "Open");
            annaUngar.barsScore = new EventScore(9.9);
            annaUngar.beamScore = new EventScore(9.65);
            annaUngar.floorScore = new EventScore(9.75);
            annaUngar.vaultScore = new EventScore(9.6);
            Meet.AddGymnast(annaUngar);

            spencerLetzler = new Gymnast("Spencer", "Letzler", dreamTeam, "Open");
            spencerLetzler.barsScore = new EventScore(2.125);
            spencerLetzler.beamScore = new EventScore(2.4);
            spencerLetzler.floorScore = new EventScore(2.9);
            spencerLetzler.vaultScore = new EventScore(2.0);
            Meet.AddGymnast(spencerLetzler);

            margaretThatcher = new Gymnast("Margaret", "Thatcher", dreamTeam, "Open");
            Meet.AddGymnast(margaretThatcher);

            soniaGandhi = new Gymnast("Sonia", "Ghandi", dreamTeam, "Open");
            Meet.AddGymnast(soniaGandhi);

            maryLouRetton = new Gymnast("Mary Lou", "Retton", olympicTypes, "Open");
            maryLouRetton.barsScore = new EventScore(8.0);
            maryLouRetton.beamScore = new EventScore(8.475);
            maryLouRetton.floorScore = new EventScore(8.925);
            maryLouRetton.vaultScore = new EventScore(8.05);
            Meet.AddGymnast(maryLouRetton);

            shannonMiller = new Gymnast("Shannon", "Miller", olympicTypes, "Open");
            shannonMiller.barsScore = new EventScore(8.75);
            shannonMiller.beamScore = new EventScore(8.8);
            shannonMiller.floorScore = new EventScore(8.95);
            shannonMiller.vaultScore = new EventScore(9.25);
            Meet.AddGymnast(shannonMiller);

            dominiqueDawes = new Gymnast("Dominique", "Dawes", olympicTypes, "Open");
            Meet.AddGymnast(dominiqueDawes);

            nastyaLiukin = new Gymnast("Nastya", "Liukin", olympicTypes, "Open");
            Meet.AddGymnast(nastyaLiukin);

            gabbyDouglas = new Gymnast("Gabby", "Douglas", olympicTypes, "Open");
            Meet.AddGymnast(gabbyDouglas);

            alyRaisman = new Gymnast("Aly", "Raisman", olympicTypes, "Open");
            Meet.AddGymnast(alyRaisman);

            simoneBiles = new Gymnast("Simone", "Biles", olympicTypes, "Open");
            Meet.AddGymnast(simoneBiles);

            bettyFlipsalot = new Gymnast("Betty", "Flipsalot", imaginaryWonders, "Open");
            bettyFlipsalot.barsScore = new EventScore(8.35);
            bettyFlipsalot.beamScore = new EventScore(8.7);
            bettyFlipsalot.floorScore = new EventScore(9.15);
            bettyFlipsalot.vaultScore = new EventScore(9.625);
            Meet.AddGymnast(bettyFlipsalot);

            tinaTwister = new Gymnast("Tina", "Twister", imaginaryWonders, "Open");
            tinaTwister.barsScore = new EventScore(8.75);
            tinaTwister.beamScore = new EventScore(8.5);
            tinaTwister.floorScore = new EventScore(8.4);
            tinaTwister.vaultScore = new EventScore(8.8);
            Meet.AddGymnast(tinaTwister);

            sandraSpinCycleOfDoom = new Gymnast("Sandra", "Spin Cycle of Doom", imaginaryWonders, "Open");
            sandraSpinCycleOfDoom.barsScore = new EventScore(6.7);
            sandraSpinCycleOfDoom.beamScore = new EventScore(4.25);
            sandraSpinCycleOfDoom.floorScore = new EventScore(5.8);
            sandraSpinCycleOfDoom.vaultScore = new EventScore(7.9);
            Meet.AddGymnast(sandraSpinCycleOfDoom);

            jumpyWombats = new Gymnast("Jumpy", "Wombats", imaginaryWonders, "Open");
            jumpyWombats.barsScore = new EventScore(3.4);
            jumpyWombats.beamScore = new EventScore(5.25);
            jumpyWombats.floorScore = new EventScore(3.9);
            jumpyWombats.vaultScore = new EventScore(8.4);
            Meet.AddGymnast(jumpyWombats);

            hildaOopssplat = new Gymnast("Hilda", "Oopssplat", imaginaryWonders, "Open");
            hildaOopssplat.barsScore = new EventScore(2.5);
            hildaOopssplat.beamScore = new EventScore(3.2);
            hildaOopssplat.floorScore = new EventScore(3.3);
            hildaOopssplat.vaultScore = new EventScore(2.8);
            Meet.AddGymnast(hildaOopssplat);

            flexiRollerball = new Gymnast("Flexi", "Rollerball", imaginaryWonders, "Open");
            flexiRollerball.barsScore = new EventScore(3.5);
            flexiRollerball.beamScore = new EventScore(4.2);
            flexiRollerball.floorScore = new EventScore(4.3);
            flexiRollerball.vaultScore = new EventScore(3.8);
            Meet.AddGymnast(flexiRollerball);

            eferEffort = new Gymnast("Efer", "Effort", imaginaryWonders, "Open");
            eferEffort.barsScore = new EventScore(1.4);
            eferEffort.beamScore = new EventScore(1.3);
            eferEffort.floorScore = new EventScore(1.7);
            eferEffort.vaultScore = new EventScore(1.9);
            Meet.AddGymnast(eferEffort);

            testMeetFirstRoster = Environment.NewLine + "01: Stephanie Gundel (101) - Dream Team" + Environment.NewLine;
            testMeetFirstRoster += "02: Lynnea Common (102) - Dream Team" + Environment.NewLine;
            testMeetFirstRoster += "03: Anna Ungar (103) - Dream Team" + Environment.NewLine;
            testMeetFirstRoster += "04: Spencer Letzler (104) - Dream Team" + Environment.NewLine;
            testMeetFirstRoster += "05: Margaret Thatcher (105) - Dream Team" + Environment.NewLine;
            testMeetFirstRoster += "06: Sonia Ghandi (106) - Dream Team" + Environment.NewLine;
            testMeetFirstRoster += "07: Mary Lou Retton (201) - Olympic Types" + Environment.NewLine;
            testMeetFirstRoster += "08: Shannon Miller (202) - Olympic Types" + Environment.NewLine;
            testMeetFirstRoster += "09: Dominique Dawes (203) - Olympic Types" + Environment.NewLine;
            testMeetFirstRoster += "10: Nastya Liukin (204) - Olympic Types" + Environment.NewLine;
            testMeetFirstRoster += "11: Gabby Douglas (205) - Olympic Types" + Environment.NewLine;
            testMeetFirstRoster += "12: Aly Raisman (206) - Olympic Types" + Environment.NewLine;
            testMeetFirstRoster += "13: Simone Biles (207) - Olympic Types" + Environment.NewLine;
            testMeetFirstRoster += "14: Betty Flipsalot (301) - Imaginary Wonders" + Environment.NewLine;
            testMeetFirstRoster += "15: Tina Twister (302) - Imaginary Wonders" + Environment.NewLine;
            testMeetFirstRoster += "16: Sandra Spin Cycle of Doom (303) - Imaginary Wonders" + Environment.NewLine;
            testMeetFirstRoster += "17: Jumpy Wombats (304) - Imaginary Wonders" + Environment.NewLine;
            testMeetFirstRoster += "18: Hilda Oopssplat (305) - Imaginary Wonders" + Environment.NewLine;
            testMeetFirstRoster += "19: Flexi Rollerball (306) - Imaginary Wonders" + Environment.NewLine;
            testMeetFirstRoster += "20: Efer Effort (307) - Imaginary Wonders" + Environment.NewLine;

            testMeetSecondRoster = Environment.NewLine + "01: Stephanie Gundel (101) - Dream Team" + Environment.NewLine;
            testMeetSecondRoster += "02: Lynnea Common (102) - Dream Team" + Environment.NewLine;
            testMeetSecondRoster += "03: Anna Ungar (103) - Dream Team" + Environment.NewLine;
            testMeetSecondRoster += "04: Spencer Letzler (104) - Dream Team" + Environment.NewLine;
            testMeetSecondRoster += "05: Margaret Thatcher (105) - Dream Team" + Environment.NewLine;
            testMeetSecondRoster += "06: Sonia Ghandi (106) - Dream Team" + Environment.NewLine;
            testMeetSecondRoster += "07: Mary Lou Retton (201) - Olympic Types" + Environment.NewLine;
            testMeetSecondRoster += "08: Shannon Miller (202) - Olympic Types" + Environment.NewLine;
            testMeetSecondRoster += "09: Dominique Dawes (203) - Olympic Types" + Environment.NewLine;
            testMeetSecondRoster += "10: Nastya Liukin (204) - Olympic Types" + Environment.NewLine;
            testMeetSecondRoster += "11: Gabby Douglas (205) - Olympic Types" + Environment.NewLine;
            testMeetSecondRoster += "12: Aly Raisman (206) - Olympic Types" + Environment.NewLine;
            testMeetSecondRoster += "13: Simone Biles (207) - Olympic Types" + Environment.NewLine;
            testMeetSecondRoster += "14: Betty Flipsalot (301) - Imaginary Wonders" + Environment.NewLine;
            testMeetSecondRoster += "15: Tina Twister (302) - Imaginary Wonders" + Environment.NewLine;
            testMeetSecondRoster += "16: Sandra Spin Cycle of Doom (303) - Imaginary Wonders" + Environment.NewLine;
            testMeetSecondRoster += "17: Jumpy Wombats (304) - Imaginary Wonders" + Environment.NewLine;
            testMeetSecondRoster += "18: Hilda Oopssplat (305) - Imaginary Wonders" + Environment.NewLine;
            testMeetSecondRoster += "19: Flexi Rollerball (306) - Imaginary Wonders" + Environment.NewLine;
            testMeetSecondRoster += "20: Efer Effort (307) - Imaginary Wonders" + Environment.NewLine;
            testMeetSecondRoster += "21: test gymnast 1 (308) - Imaginary Wonders" + Environment.NewLine;
            testMeetSecondRoster += "22: test gymnast 2 (309) - Imaginary Wonders" + Environment.NewLine;
            testMeetSecondRoster += "23: test gymnast 3 (310) - Imaginary Wonders" + Environment.NewLine;

            testMeetTestDivisionRoster += Environment.NewLine;
            testMeetTestDivisionRoster += "1: test gymnast 1 (308) - Imaginary Wonders" + Environment.NewLine;
            testMeetTestDivisionRoster += "2: test gymnast 2 (309) - Imaginary Wonders" + Environment.NewLine;
            testMeetTestDivisionRoster += "3: test gymnast 3 (310) - Imaginary Wonders" + Environment.NewLine;

            testMeetBarsStandings = Environment.NewLine;
            testMeetBarsStandings += "01:  9.9   >> Anna Ungar (103) - Dream Team" + Environment.NewLine;
            testMeetBarsStandings += "02:  9.85  >> Stephanie Gundel (101) - Dream Team" + Environment.NewLine;
            testMeetBarsStandings += "03:  9.8   >> Lynnea Common (102) - Dream Team" + Environment.NewLine;
            testMeetBarsStandings += "04:  8.75  >> Shannon Miller (202) - Olympic Types" + Environment.NewLine;
            testMeetBarsStandings += "05:  8.75  >> Tina Twister (302) - Imaginary Wonders" + Environment.NewLine;
            testMeetBarsStandings += "06:  8.35  >> Betty Flipsalot (301) - Imaginary Wonders" + Environment.NewLine;
            testMeetBarsStandings += "07:  8.0   >> Mary Lou Retton (201) - Olympic Types" + Environment.NewLine;
            testMeetBarsStandings += "08:  6.7   >> Sandra Spin Cycle of Doom (303) - Imaginary Wonders" + Environment.NewLine;
            testMeetBarsStandings += "09:  3.5   >> Flexi Rollerball (306) - Imaginary Wonders" + Environment.NewLine;
            testMeetBarsStandings += "10:  3.4   >> Jumpy Wombats (304) - Imaginary Wonders" + Environment.NewLine;
            testMeetBarsStandings += "11:  2.5   >> Hilda Oopssplat (305) - Imaginary Wonders" + Environment.NewLine;
            testMeetBarsStandings += "12:  2.125 >> Spencer Letzler (104) - Dream Team" + Environment.NewLine;
            testMeetBarsStandings += "13:  1.4   >> Efer Effort (307) - Imaginary Wonders" + Environment.NewLine;

            testMeetBeamStandings = Environment.NewLine;
            testMeetBeamStandings += "01:  9.75  >> Lynnea Common (102) - Dream Team" + Environment.NewLine;
            testMeetBeamStandings += "02:  9.7   >> Stephanie Gundel (101) - Dream Team" + Environment.NewLine;
            testMeetBeamStandings += "03:  9.65  >> Anna Ungar (103) - Dream Team" + Environment.NewLine;
            testMeetBeamStandings += "04:  8.8   >> Shannon Miller (202) - Olympic Types" + Environment.NewLine;
            testMeetBeamStandings += "05:  8.7   >> Betty Flipsalot (301) - Imaginary Wonders" + Environment.NewLine;
            testMeetBeamStandings += "06:  8.5   >> Tina Twister (302) - Imaginary Wonders" + Environment.NewLine;
            testMeetBeamStandings += "07:  8.475 >> Mary Lou Retton (201) - Olympic Types" + Environment.NewLine;
            testMeetBeamStandings += "08:  5.25  >> Jumpy Wombats (304) - Imaginary Wonders" + Environment.NewLine;
            testMeetBeamStandings += "09:  4.25  >> Sandra Spin Cycle of Doom (303) - Imaginary Wonders" + Environment.NewLine;
            testMeetBeamStandings += "10:  4.2   >> Flexi Rollerball (306) - Imaginary Wonders" + Environment.NewLine;
            testMeetBeamStandings += "11:  3.2   >> Hilda Oopssplat (305) - Imaginary Wonders" + Environment.NewLine;
            testMeetBeamStandings += "12:  2.4   >> Spencer Letzler (104) - Dream Team" + Environment.NewLine;
            testMeetBeamStandings += "13:  1.3   >> Efer Effort (307) - Imaginary Wonders" + Environment.NewLine;

            testMeetFloorStandings = Environment.NewLine;
            testMeetFloorStandings += "01:  9.75  >> Anna Ungar (103) - Dream Team" + Environment.NewLine;
            testMeetFloorStandings += "02:  9.7   >> Lynnea Common (102) - Dream Team" + Environment.NewLine;
            testMeetFloorStandings += "03:  9.65  >> Stephanie Gundel (101) - Dream Team" + Environment.NewLine;
            testMeetFloorStandings += "04:  9.15  >> Betty Flipsalot (301) - Imaginary Wonders" + Environment.NewLine;
            testMeetFloorStandings += "05:  8.95  >> Shannon Miller (202) - Olympic Types" + Environment.NewLine;
            testMeetFloorStandings += "06:  8.925 >> Mary Lou Retton (201) - Olympic Types" + Environment.NewLine;
            testMeetFloorStandings += "07:  8.4   >> Tina Twister (302) - Imaginary Wonders" + Environment.NewLine;
            testMeetFloorStandings += "08:  5.8   >> Sandra Spin Cycle of Doom (303) - Imaginary Wonders" + Environment.NewLine;
            testMeetFloorStandings += "09:  4.3   >> Flexi Rollerball (306) - Imaginary Wonders" + Environment.NewLine;
            testMeetFloorStandings += "10:  3.9   >> Jumpy Wombats (304) - Imaginary Wonders" + Environment.NewLine;
            testMeetFloorStandings += "11:  3.3   >> Hilda Oopssplat (305) - Imaginary Wonders" + Environment.NewLine;
            testMeetFloorStandings += "12:  2.9   >> Spencer Letzler (104) - Dream Team" + Environment.NewLine;
            testMeetFloorStandings += "13:  1.7   >> Efer Effort (307) - Imaginary Wonders" + Environment.NewLine;

            testMeetVaultStandings = Environment.NewLine;
            testMeetVaultStandings += "01:  9.95  >> Stephanie Gundel (101) - Dream Team" + Environment.NewLine;
            testMeetVaultStandings += "02:  9.625 >> Betty Flipsalot (301) - Imaginary Wonders" + Environment.NewLine;
            testMeetVaultStandings += "03:  9.6   >> Anna Ungar (103) - Dream Team" + Environment.NewLine;
            testMeetVaultStandings += "04:  9.55  >> Lynnea Common (102) - Dream Team" + Environment.NewLine;
            testMeetVaultStandings += "05:  9.25  >> Shannon Miller (202) - Olympic Types" + Environment.NewLine;
            testMeetVaultStandings += "06:  8.8   >> Tina Twister (302) - Imaginary Wonders" + Environment.NewLine;
            testMeetVaultStandings += "07:  8.4   >> Jumpy Wombats (304) - Imaginary Wonders" + Environment.NewLine;
            testMeetVaultStandings += "08:  8.05  >> Mary Lou Retton (201) - Olympic Types" + Environment.NewLine;
            testMeetVaultStandings += "09:  7.9   >> Sandra Spin Cycle of Doom (303) - Imaginary Wonders" + Environment.NewLine;
            testMeetVaultStandings += "10:  3.8   >> Flexi Rollerball (306) - Imaginary Wonders" + Environment.NewLine;
            testMeetVaultStandings += "11:  2.8   >> Hilda Oopssplat (305) - Imaginary Wonders" + Environment.NewLine;
            testMeetVaultStandings += "12:  2.0   >> Spencer Letzler (104) - Dream Team" + Environment.NewLine;
            testMeetVaultStandings += "13:  1.9   >> Efer Effort (307) - Imaginary Wonders" + Environment.NewLine;

            testMeetOverallStandings = Environment.NewLine;
            testMeetOverallStandings += "01: 39.15  >> Stephanie Gundel (101) - Dream Team" + Environment.NewLine;
            testMeetOverallStandings += "02: 38.9   >> Anna Ungar (103) - Dream Team" + Environment.NewLine;
            testMeetOverallStandings += "03: 38.8   >> Lynnea Common (102) - Dream Team" + Environment.NewLine;
            testMeetOverallStandings += "04: 35.825 >> Betty Flipsalot (301) - Imaginary Wonders" + Environment.NewLine;
            testMeetOverallStandings += "05: 35.75  >> Shannon Miller (202) - Olympic Types" + Environment.NewLine;
            testMeetOverallStandings += "06: 34.45  >> Tina Twister (302) - Imaginary Wonders" + Environment.NewLine;
            testMeetOverallStandings += "07: 33.45  >> Mary Lou Retton (201) - Olympic Types" + Environment.NewLine;
            testMeetOverallStandings += "08: 24.65  >> Sandra Spin Cycle of Doom (303) - Imaginary Wonders" + Environment.NewLine;
            testMeetOverallStandings += "09: 20.95  >> Jumpy Wombats (304) - Imaginary Wonders" + Environment.NewLine;
            testMeetOverallStandings += "10: 15.8   >> Flexi Rollerball (306) - Imaginary Wonders" + Environment.NewLine;
            testMeetOverallStandings += "11: 11.8   >> Hilda Oopssplat (305) - Imaginary Wonders" + Environment.NewLine;
            testMeetOverallStandings += "12:  9.425 >> Spencer Letzler (104) - Dream Team" + Environment.NewLine;
            testMeetOverallStandings += "13:  6.3   >> Efer Effort (307) - Imaginary Wonders" + Environment.NewLine;

            testMeetUpdatedRoster = Environment.NewLine;
            testMeetUpdatedRoster += "01: Stephanie Gundel (101) - Dream Team" + Environment.NewLine;
            testMeetUpdatedRoster += "02: Lynnea Common (102) - Dream Team" + Environment.NewLine;
            testMeetUpdatedRoster += "03: Anna Ungar (103) - Dream Team" + Environment.NewLine;
            testMeetUpdatedRoster += "04: Spencer Letzler (104) - Dream Team" + Environment.NewLine;
            testMeetUpdatedRoster += "05: Margaret Thatcher (105) - Dream Team" + Environment.NewLine;
            testMeetUpdatedRoster += "06: Sonia Ghandi (106) - Dream Team" + Environment.NewLine;
            testMeetUpdatedRoster += "07: Mary Lou Retton (201) - Olympic Types" + Environment.NewLine;
            testMeetUpdatedRoster += "08: Shannon Miller (202) - Olympic Types" + Environment.NewLine;
            testMeetUpdatedRoster += "09: Dominique Dawes (203) - Olympic Types" + Environment.NewLine;
            testMeetUpdatedRoster += "10: Nastya Liukin (204) - Olympic Types" + Environment.NewLine;
            testMeetUpdatedRoster += "11: Gabby Douglas (205) - Olympic Types" + Environment.NewLine;
            testMeetUpdatedRoster += "12: Aly Raisman (206) - Olympic Types" + Environment.NewLine;
            testMeetUpdatedRoster += "13: Simone Biles (207) - Olympic Types" + Environment.NewLine;
            testMeetUpdatedRoster += "14: Laurie Hernandez (208) - Olympic Types" + Environment.NewLine;
            testMeetUpdatedRoster += "15: Madison Kocian (209) - Olympic Types" + Environment.NewLine;
            testMeetUpdatedRoster += "16: Betty Flipsalot (301) - Imaginary Wonders" + Environment.NewLine;
            testMeetUpdatedRoster += "17: Tina Twister (302) - Imaginary Wonders" + Environment.NewLine;
            testMeetUpdatedRoster += "18: Sandra Spin Cycle of Doom (303) - Imaginary Wonders" + Environment.NewLine;
            testMeetUpdatedRoster += "19: Jumpy Wombats (304) - Imaginary Wonders" + Environment.NewLine;
            testMeetUpdatedRoster += "20: Hilda Oopssplat (305) - Imaginary Wonders" + Environment.NewLine;
            testMeetUpdatedRoster += "21: Flexi Rollerball (306) - Imaginary Wonders" + Environment.NewLine;
            testMeetUpdatedRoster += "22: Efer Effort (307) - Imaginary Wonders" + Environment.NewLine;
            testMeetUpdatedRoster += "23: Wendy Whiplash (308) - Imaginary Wonders" + Environment.NewLine;

            testMeetUpdatedTestDivisionRoster = Environment.NewLine;
            testMeetUpdatedTestDivisionRoster += "1: Laurie Hernandez (208) - Olympic Types" + Environment.NewLine;
            testMeetUpdatedTestDivisionRoster += "2: Madison Kocian (209) - Olympic Types" + Environment.NewLine;
            testMeetUpdatedTestDivisionRoster += "3: Wendy Whiplash (308) - Imaginary Wonders" + Environment.NewLine;

            testMeetUpdatedBarsStandings = Environment.NewLine;
            testMeetUpdatedBarsStandings += "01:  9.9   >> Anna Ungar (103) - Dream Team" + Environment.NewLine;
            testMeetUpdatedBarsStandings += "02:  9.9   >> Madison Kocian (209) - Olympic Types" + Environment.NewLine;
            testMeetUpdatedBarsStandings += "03:  9.85  >> Stephanie Gundel (101) - Dream Team" + Environment.NewLine;
            testMeetUpdatedBarsStandings += "04:  9.8   >> Lynnea Common (102) - Dream Team" + Environment.NewLine;
            testMeetUpdatedBarsStandings += "05:  9.3   >> Laurie Hernandez (208) - Olympic Types" + Environment.NewLine;
            testMeetUpdatedBarsStandings += "06:  8.75  >> Shannon Miller (202) - Olympic Types" + Environment.NewLine;
            testMeetUpdatedBarsStandings += "07:  8.75  >> Tina Twister (302) - Imaginary Wonders" + Environment.NewLine;
            testMeetUpdatedBarsStandings += "08:  8.35  >> Betty Flipsalot (301) - Imaginary Wonders" + Environment.NewLine;
            testMeetUpdatedBarsStandings += "09:  8.0   >> Mary Lou Retton (201) - Olympic Types" + Environment.NewLine;
            testMeetUpdatedBarsStandings += "10:  6.7   >> Sandra Spin Cycle of Doom (303) - Imaginary Wonders" + Environment.NewLine;
            testMeetUpdatedBarsStandings += "11:  3.5   >> Flexi Rollerball (306) - Imaginary Wonders" + Environment.NewLine;
            testMeetUpdatedBarsStandings += "12:  3.4   >> Jumpy Wombats (304) - Imaginary Wonders" + Environment.NewLine;
            testMeetUpdatedBarsStandings += "13:  2.775 >> Wendy Whiplash (308) - Imaginary Wonders" + Environment.NewLine;
            testMeetUpdatedBarsStandings += "14:  2.5   >> Hilda Oopssplat (305) - Imaginary Wonders" + Environment.NewLine;
            testMeetUpdatedBarsStandings += "15:  2.125 >> Spencer Letzler (104) - Dream Team" + Environment.NewLine;
            testMeetUpdatedBarsStandings += "16:  1.4   >> Efer Effort (307) - Imaginary Wonders" + Environment.NewLine;

            testMeetUpdatedBeamStandings = Environment.NewLine;
            testMeetUpdatedBeamStandings += "01:  9.75  >> Lynnea Common (102) - Dream Team" + Environment.NewLine;
            testMeetUpdatedBeamStandings += "02:  9.7   >> Stephanie Gundel (101) - Dream Team" + Environment.NewLine;
            testMeetUpdatedBeamStandings += "03:  9.65  >> Anna Ungar (103) - Dream Team" + Environment.NewLine;
            testMeetUpdatedBeamStandings += "04:  9.2   >> Laurie Hernandez (208) - Olympic Types" + Environment.NewLine;
            testMeetUpdatedBeamStandings += "05:  9.0   >> Madison Kocian (209) - Olympic Types" + Environment.NewLine;
            testMeetUpdatedBeamStandings += "06:  8.8   >> Shannon Miller (202) - Olympic Types" + Environment.NewLine;
            testMeetUpdatedBeamStandings += "07:  8.7   >> Betty Flipsalot (301) - Imaginary Wonders" + Environment.NewLine;
            testMeetUpdatedBeamStandings += "08:  8.5   >> Tina Twister (302) - Imaginary Wonders" + Environment.NewLine;
            testMeetUpdatedBeamStandings += "09:  8.475 >> Mary Lou Retton (201) - Olympic Types" + Environment.NewLine;
            testMeetUpdatedBeamStandings += "10:  5.25  >> Jumpy Wombats (304) - Imaginary Wonders" + Environment.NewLine;
            testMeetUpdatedBeamStandings += "11:  4.25  >> Sandra Spin Cycle of Doom (303) - Imaginary Wonders" + Environment.NewLine;
            testMeetUpdatedBeamStandings += "12:  4.2   >> Flexi Rollerball (306) - Imaginary Wonders" + Environment.NewLine;
            testMeetUpdatedBeamStandings += "13:  3.775 >> Wendy Whiplash (308) - Imaginary Wonders" + Environment.NewLine;
            testMeetUpdatedBeamStandings += "14:  3.2   >> Hilda Oopssplat (305) - Imaginary Wonders" + Environment.NewLine;
            testMeetUpdatedBeamStandings += "15:  2.4   >> Spencer Letzler (104) - Dream Team" + Environment.NewLine;
            testMeetUpdatedBeamStandings += "16:  1.3   >> Efer Effort (307) - Imaginary Wonders" + Environment.NewLine;

            testMeetUpdatedFloorStandings = Environment.NewLine;
            testMeetUpdatedFloorStandings += "01:  9.75  >> Anna Ungar (103) - Dream Team" + Environment.NewLine;
            testMeetUpdatedFloorStandings += "02:  9.7   >> Lynnea Common (102) - Dream Team" + Environment.NewLine;
            testMeetUpdatedFloorStandings += "03:  9.65  >> Stephanie Gundel (101) - Dream Team" + Environment.NewLine;
            testMeetUpdatedFloorStandings += "04:  9.4   >> Laurie Hernandez (208) - Olympic Types" + Environment.NewLine;
            testMeetUpdatedFloorStandings += "05:  9.15  >> Betty Flipsalot (301) - Imaginary Wonders" + Environment.NewLine;
            testMeetUpdatedFloorStandings += "06:  8.95  >> Shannon Miller (202) - Olympic Types" + Environment.NewLine;
            testMeetUpdatedFloorStandings += "07:  8.925 >> Mary Lou Retton (201) - Olympic Types" + Environment.NewLine;
            testMeetUpdatedFloorStandings += "08:  8.4   >> Tina Twister (302) - Imaginary Wonders" + Environment.NewLine;
            testMeetUpdatedFloorStandings += "09:  5.8   >> Sandra Spin Cycle of Doom (303) - Imaginary Wonders" + Environment.NewLine;
            testMeetUpdatedFloorStandings += "10:  4.3   >> Flexi Rollerball (306) - Imaginary Wonders" + Environment.NewLine;
            testMeetUpdatedFloorStandings += "11:  3.9   >> Jumpy Wombats (304) - Imaginary Wonders" + Environment.NewLine;
            testMeetUpdatedFloorStandings += "12:  3.3   >> Hilda Oopssplat (305) - Imaginary Wonders" + Environment.NewLine;
            testMeetUpdatedFloorStandings += "13:  2.9   >> Spencer Letzler (104) - Dream Team" + Environment.NewLine;
            testMeetUpdatedFloorStandings += "14:  1.775 >> Wendy Whiplash (308) - Imaginary Wonders" + Environment.NewLine;
            testMeetUpdatedFloorStandings += "15:  1.7   >> Efer Effort (307) - Imaginary Wonders" + Environment.NewLine;

            testMeetUpdatedVaultStandings = Environment.NewLine;
            testMeetUpdatedVaultStandings += "01:  9.95  >> Stephanie Gundel (101) - Dream Team" + Environment.NewLine;
            testMeetUpdatedVaultStandings += "02:  9.625 >> Betty Flipsalot (301) - Imaginary Wonders" + Environment.NewLine;
            testMeetUpdatedVaultStandings += "03:  9.6   >> Anna Ungar (103) - Dream Team" + Environment.NewLine;
            testMeetUpdatedVaultStandings += "04:  9.55  >> Lynnea Common (102) - Dream Team" + Environment.NewLine;
            testMeetUpdatedVaultStandings += "05:  9.25  >> Shannon Miller (202) - Olympic Types" + Environment.NewLine;
            testMeetUpdatedVaultStandings += "06:  9.1   >> Laurie Hernandez (208) - Olympic Types" + Environment.NewLine;
            testMeetUpdatedVaultStandings += "07:  9.0   >> Madison Kocian (209) - Olympic Types" + Environment.NewLine;
            testMeetUpdatedVaultStandings += "08:  8.8   >> Tina Twister (302) - Imaginary Wonders" + Environment.NewLine;
            testMeetUpdatedVaultStandings += "09:  8.4   >> Jumpy Wombats (304) - Imaginary Wonders" + Environment.NewLine;
            testMeetUpdatedVaultStandings += "10:  8.05  >> Mary Lou Retton (201) - Olympic Types" + Environment.NewLine;
            testMeetUpdatedVaultStandings += "11:  7.9   >> Sandra Spin Cycle of Doom (303) - Imaginary Wonders" + Environment.NewLine;
            testMeetUpdatedVaultStandings += "12:  3.8   >> Flexi Rollerball (306) - Imaginary Wonders" + Environment.NewLine;
            testMeetUpdatedVaultStandings += "13:  2.8   >> Hilda Oopssplat (305) - Imaginary Wonders" + Environment.NewLine;
            testMeetUpdatedVaultStandings += "14:  2.0   >> Spencer Letzler (104) - Dream Team" + Environment.NewLine;
            testMeetUpdatedVaultStandings += "15:  1.9   >> Efer Effort (307) - Imaginary Wonders" + Environment.NewLine;

            testMeetUpdatedOverallStandings = Environment.NewLine;
            testMeetUpdatedOverallStandings += "01: 39.15  >> Stephanie Gundel (101) - Dream Team" + Environment.NewLine;
            testMeetUpdatedOverallStandings += "02: 38.9   >> Anna Ungar (103) - Dream Team" + Environment.NewLine;
            testMeetUpdatedOverallStandings += "03: 38.8   >> Lynnea Common (102) - Dream Team" + Environment.NewLine;
            testMeetUpdatedOverallStandings += "04: 37.0   >> Laurie Hernandez (208) - Olympic Types" + Environment.NewLine;
            testMeetUpdatedOverallStandings += "05: 35.825 >> Betty Flipsalot (301) - Imaginary Wonders" + Environment.NewLine;
            testMeetUpdatedOverallStandings += "06: 35.75  >> Shannon Miller (202) - Olympic Types" + Environment.NewLine;
            testMeetUpdatedOverallStandings += "07: 34.45  >> Tina Twister (302) - Imaginary Wonders" + Environment.NewLine;
            testMeetUpdatedOverallStandings += "08: 33.45  >> Mary Lou Retton (201) - Olympic Types" + Environment.NewLine;
            testMeetUpdatedOverallStandings += "09: 27.9   >> Madison Kocian (209) - Olympic Types" + Environment.NewLine;
            testMeetUpdatedOverallStandings += "10: 24.65  >> Sandra Spin Cycle of Doom (303) - Imaginary Wonders" + Environment.NewLine;
            testMeetUpdatedOverallStandings += "11: 20.95  >> Jumpy Wombats (304) - Imaginary Wonders" + Environment.NewLine;
            testMeetUpdatedOverallStandings += "12: 15.8   >> Flexi Rollerball (306) - Imaginary Wonders" + Environment.NewLine;
            testMeetUpdatedOverallStandings += "13: 11.8   >> Hilda Oopssplat (305) - Imaginary Wonders" + Environment.NewLine;
            testMeetUpdatedOverallStandings += "14:  9.425 >> Spencer Letzler (104) - Dream Team" + Environment.NewLine;
            testMeetUpdatedOverallStandings += "15:  8.325 >> Wendy Whiplash (308) - Imaginary Wonders" + Environment.NewLine;
            testMeetUpdatedOverallStandings += "16:  6.3   >> Efer Effort (307) - Imaginary Wonders" + Environment.NewLine;

            testMeetUpdatedBarsStandings_TestDivision = Environment.NewLine;
            testMeetUpdatedBarsStandings_TestDivision += "1:  9.9   >> Madison Kocian (209) - Olympic Types" + Environment.NewLine;
            testMeetUpdatedBarsStandings_TestDivision += "2:  9.3   >> Laurie Hernandez (208) - Olympic Types" + Environment.NewLine;
            testMeetUpdatedBarsStandings_TestDivision += "3:  2.775 >> Wendy Whiplash (308) - Imaginary Wonders" + Environment.NewLine;

            testMeetUpdatedBeamStandings_TestDivision = Environment.NewLine;
            testMeetUpdatedBeamStandings_TestDivision += "1:  9.2   >> Laurie Hernandez (208) - Olympic Types" + Environment.NewLine;
            testMeetUpdatedBeamStandings_TestDivision += "2:  9.0   >> Madison Kocian (209) - Olympic Types" + Environment.NewLine;
            testMeetUpdatedBeamStandings_TestDivision += "3:  3.775 >> Wendy Whiplash (308) - Imaginary Wonders" + Environment.NewLine;

            testMeetUpdatedFloorStandings_TestDivision = Environment.NewLine;
            testMeetUpdatedFloorStandings_TestDivision += "1:  9.4   >> Laurie Hernandez (208) - Olympic Types" + Environment.NewLine;
            testMeetUpdatedFloorStandings_TestDivision += "2:  1.775 >> Wendy Whiplash (308) - Imaginary Wonders" + Environment.NewLine;

            testMeetUpdatedVaultStandings_TestDivision = Environment.NewLine;
            testMeetUpdatedVaultStandings_TestDivision += "1:  9.1   >> Laurie Hernandez (208) - Olympic Types" + Environment.NewLine;
            testMeetUpdatedVaultStandings_TestDivision += "2:  9.0   >> Madison Kocian (209) - Olympic Types" + Environment.NewLine;

            testMeetUpdatedOverallStandings_TestDivision = Environment.NewLine;
            testMeetUpdatedOverallStandings_TestDivision += "1: 37.0   >> Laurie Hernandez (208) - Olympic Types" + Environment.NewLine;
            testMeetUpdatedOverallStandings_TestDivision += "2: 27.9   >> Madison Kocian (209) - Olympic Types" + Environment.NewLine;
            testMeetUpdatedOverallStandings_TestDivision += "3:  8.325 >> Wendy Whiplash (308) - Imaginary Wonders" + Environment.NewLine;
        }


        // TESTING EVENT SCORE: Focus on operation of nullable<T> and overloaded operators
        #region Testing_EventScore.
        [TestMethod]
        public void TestMethod001() {
            string caughtMessage = ProgramConstants.NO_EXCEPTION_MSG;
            double? score = 10.1;

            EventScore testScore = new EventScore();
            try {
                testScore = new EventScore(score);
            }
            catch (Exception_ScoreOutOfValidRange e) {
                caughtMessage = e.Message;
            }

            Assert.AreEqual<string>("EXCEPTION: score of " + score.ToString() + " not within valid range of " + Meet.meetParameters.minScore.ToString() 
                + " to " + Meet.meetParameters.maxScore.ToString(), caughtMessage);
            Assert.AreEqual<double?>(new double?(), testScore.score);
        }

        [TestMethod]
        public void TestMethod002() {
            string caughtMessage = "";
            double score = -0.1;

            EventScore testScore = new EventScore(null);
            try {
                testScore = new EventScore(score);
            }
            catch (Exception_ScoreOutOfValidRange e) {
                caughtMessage = e.Message;
            }

            Assert.AreEqual<string>("EXCEPTION: score of " + score.ToString() + " not within valid range of " + Meet.meetParameters.minScore.ToString() 
                + " to " + Meet.meetParameters.maxScore.ToString(), caughtMessage);
            Assert.AreEqual<double?>(new double?(), testScore.score);
        }

        [TestMethod]
        public void TestMethod003() {
            string caughtMessage = "";
            double score = 9.1;

            EventScore testScore = new EventScore(null);
            try {
                testScore = new EventScore(score);
            }
            catch (Exception_ScoreOutOfValidRange e) {
                caughtMessage = e.Message;
            }

            Assert.AreEqual<string>("", caughtMessage);
            Assert.AreEqual<double?>(new double?(9.1), testScore.score);
        }

        [TestMethod]
        public void TestMethod004() {
            double testScore = 7.35;
            EventScore score01 = new EventScore(testScore);
            EventScore score02 = new EventScore(null);
            double? addedScores = score01 + score02;
            Assert.AreEqual<bool>(true, addedScores.HasValue);
            Assert.AreEqual<double>(testScore, addedScores.Value);

            score01 = new EventScore(null);
            score02 = new EventScore(testScore);
            addedScores = score01 + score02;
            Assert.AreEqual<bool>(true, addedScores.HasValue);
            Assert.AreEqual<double>(testScore, addedScores.Value);

            score01 = new EventScore(null);
            score02 = new EventScore(null);
            addedScores = score01 + score02;
            Assert.AreEqual<bool>(false, addedScores.HasValue);

            score01 = new EventScore(testScore);
            score02 = new EventScore(testScore);
            addedScores = score01 + score02;
            Assert.AreEqual<bool>(true, addedScores.HasValue);
            Assert.AreEqual<double>(testScore * 2, addedScores.Value);

        }

        [TestMethod]
        public void TestMethod005() {
            EventScore scoreHigh = new EventScore(8.35);
            EventScore scoreLow = new EventScore(5.4);
            EventScore scoreNull = new EventScore(null);
            EventScore scoreHighSame = new EventScore(8.35);

            Assert.AreEqual<bool>(true, scoreHigh > scoreLow);
            Assert.AreEqual<bool>(true, scoreHigh >= scoreLow);
            Assert.AreEqual<bool>(true, scoreHigh != scoreLow);

            Assert.AreEqual<bool>(false, scoreHigh < scoreLow);
            Assert.AreEqual<bool>(false, scoreHigh <= scoreLow);
            Assert.AreEqual<bool>(false, scoreHigh == scoreLow);

            Assert.AreEqual<bool>(false, scoreHigh > scoreHighSame);
            Assert.AreEqual<bool>(true, scoreHigh >= scoreHighSame);
            Assert.AreEqual<bool>(false, scoreHigh != scoreHighSame);

            Assert.AreEqual<bool>(false, scoreHigh < scoreHighSame);
            Assert.AreEqual<bool>(true, scoreHigh <= scoreHighSame);
            Assert.AreEqual<bool>(true, scoreHigh == scoreHighSame);

            Assert.AreEqual<bool>(true, scoreHigh > scoreNull);
            Assert.AreEqual<bool>(true, scoreHigh >= scoreNull);
            Assert.AreEqual<bool>(true, scoreHigh != scoreNull);

            Assert.AreEqual<bool>(false, scoreHigh < scoreNull);
            Assert.AreEqual<bool>(false, scoreHigh <= scoreNull);
            Assert.AreEqual<bool>(false, scoreHigh == scoreNull);

            Assert.AreEqual<bool>(false, scoreLow > scoreHigh);
            Assert.AreEqual<bool>(false, scoreLow >= scoreHigh);
            Assert.AreEqual<bool>(true, scoreLow != scoreHigh);

            Assert.AreEqual<bool>(true, scoreLow < scoreHigh);
            Assert.AreEqual<bool>(true, scoreLow <= scoreHigh);
            Assert.AreEqual<bool>(false, scoreLow == scoreHigh);

            Assert.AreEqual<bool>(true, scoreLow > scoreNull);
            Assert.AreEqual<bool>(true, scoreLow >= scoreNull);
            Assert.AreEqual<bool>(true, scoreLow != scoreNull);

            Assert.AreEqual<bool>(false, scoreLow < scoreNull);
            Assert.AreEqual<bool>(false, scoreLow <= scoreNull);
            Assert.AreEqual<bool>(false, scoreLow == scoreNull);
        }
        #endregion Testing_EventScore.


        // TESTING BASIC CLASSES of Gymnast, Team, and Division
        #region Testing_GymnastTeamDivision.

        [TestMethod]
        public void TestMethod006() {
            Assert.AreEqual<int>(4, Meet.GenerateNextTeamNumber());
            Assert.AreEqual<int>(1, dreamTeam.number);
            Assert.AreEqual<int>(2, olympicTypes.number);
            Assert.AreEqual<int>(3, imaginaryWonders.number);
        }

        [TestMethod]
        public void TestMethod007() {
            Assert.AreEqual<int>(3, Meet.TeamCount);
            Assert.AreEqual<int>(1, Meet.DivisionCount);
            Assert.AreEqual<int>(20, Meet.GymnastCount);

            Assert.AreEqual<bool>(true, Meet.ContainsTeam("Dream Team"));
            Assert.AreEqual<bool>(true, Meet.ContainsTeam("Olympic Types"));
            Assert.AreEqual<bool>(true, Meet.ContainsTeam("Imaginary Wonders"));
            Assert.AreEqual<bool>(false, Meet.ContainsTeam("Mouth Breathers"));

            Assert.AreEqual<string>("Dream Team", dreamTeam.name);
            Assert.AreEqual<bool>(true, Meet.ContainsDivision("Open"));
            Assert.AreEqual<bool>(true, Meet.ContainsTeam("Dream Team"), "string");
            Assert.AreEqual<bool>(true, Meet.ContainsTeam(dreamTeam), "team");
        }

        [TestMethod]
        public void TestMethod008() {
            Assert.AreEqual<int>(6, dreamTeam.rosterSize);
            Assert.AreEqual<int>(7, imaginaryWonders.rosterSize);

            Gymnast hooptyStinkpot = null;

            string exceptionMessage = "No exception / no message";
            try {
                hooptyStinkpot = new Gymnast("Hoopty", "Stinkpot", imaginaryWonders, "Open");
                Meet.AddGymnast(hooptyStinkpot);
            }
            catch (Exception e) {
                exceptionMessage = e.Message;
            }

            Assert.AreEqual<string>("No exception / no message", exceptionMessage);
            Assert.AreEqual<string>("Hoopty", hooptyStinkpot.firstName);
            Assert.AreEqual<string>("Stinkpot", hooptyStinkpot.lastName);
            Assert.AreEqual<string>("Imaginary Wonders", hooptyStinkpot.team.name);
            Assert.AreEqual<string>("Open", hooptyStinkpot.division.name);
            Assert.AreEqual<bool>(true, Meet.ContainsGymnast("Hoopty", "Stinkpot", "Imaginary Wonders"));

            Assert.AreEqual<int>(8, imaginaryWonders.rosterSize);
            Assert.AreEqual<int>(21, Meet.GymnastCount);

            Meet.RemoveGymnast(hooptyStinkpot);
            Assert.AreEqual<bool>(false, Meet.ContainsGymnast("Hoopty", "Stinkpot", "Imaginary Wonders"));
            Assert.AreEqual<int>(20, Meet.GymnastCount);
            Assert.AreEqual<int>(7, imaginaryWonders.rosterSize);
        }

        [TestMethod]
        public void TestMethod009() {
            Assert.AreEqual<int>(6, dreamTeam.rosterSize, "a");

            Assert.AreEqual<bool>(true, Meet.ContainsGymnast("Stephanie", "Gundel", "Dream Team"), "Stephanie Gundel Dream Team");
            Assert.AreEqual<bool>(false, Meet.ContainsGymnast("Stepahnie", "Gundel", "Imaginary Wonders"));

            Assert.AreEqual<bool>(true, Meet.ContainsDivision("Open"), "Contains Open Division");
            Assert.AreEqual<bool>(false, Meet.ContainsDivision("open"), "e");
            Assert.AreEqual<bool>(false, Meet.ContainsDivision("Macho"), "f");

        }

        [TestMethod]
        public void TestMethod010() {
            Assert.AreEqual<string>("Stephanie Gundel (101)", stephanieGundel.ToString());
            Assert.AreEqual<string>("Flexi Rollerball (306)", flexiRollerball.ToString());
        }
        #endregion Testing_GymnastTeamDivision.

        // TESTING ORDERING and FILTERING FUNCTIONS
        #region Testing_OrderingFiltering.
        [TestMethod]
        public void TestMethod011() {
            Assert.AreEqual<string>(testMeetFirstRoster, Meet.RosterString(),
                "First difference at index: " + HelperMethods.findDifference(Meet.RosterString(), testMeetFirstRoster).Item1
                + Environment.NewLine + HelperMethods.findDifference(Meet.RosterString(), testMeetFirstRoster).Item2);

            Assert.AreEqual<string>(testMeetFirstRoster, Meet.RosterString("Open"),
                "First difference at index: " + HelperMethods.findDifference(Meet.RosterString(), testMeetFirstRoster).Item1
                + Environment.NewLine + HelperMethods.findDifference(Meet.RosterString(), testMeetFirstRoster).Item2);

            Division testDivision = new Division("Test");
            Meet.AddDivision(testDivision);

            Assert.AreEqual<int>(2, Meet.DivisionCount);

            Gymnast tg1 = new Gymnast("test", "gymnast 1", imaginaryWonders, "Test");
            Meet.AddGymnast(tg1);
            Gymnast tg2 = new Gymnast("test", "gymnast 2", imaginaryWonders, "Test");
            Meet.AddGymnast(tg2);
            Gymnast tg3 = new Gymnast("test", "gymnast 3", imaginaryWonders, "Test");
            Meet.AddGymnast(tg3);

            Assert.AreEqual<string>(testMeetTestDivisionRoster, Meet.RosterString("Test"),
                "First difference at index: " + HelperMethods.findDifference(Meet.RosterString(), testMeetFirstRoster).Item1
                + Environment.NewLine + HelperMethods.findDifference(Meet.RosterString(), testMeetFirstRoster).Item2);

            Assert.AreEqual<string>(testMeetSecondRoster, Meet.RosterString(),
                "First difference at index: " + HelperMethods.findDifference(Meet.RosterString(), testMeetSecondRoster).Item1
                + Environment.NewLine + HelperMethods.findDifference(Meet.RosterString(), testMeetSecondRoster).Item2);

            Assert.AreEqual<string>(testMeetFirstRoster, Meet.RosterString("Open"),
                "First difference at index: " + HelperMethods.findDifference(Meet.RosterString(), testMeetFirstRoster).Item1
                + Environment.NewLine + HelperMethods.findDifference(Meet.RosterString(), testMeetFirstRoster).Item2);

            Meet.RemoveGymnast(tg1);
            Meet.RemoveGymnast(tg2);
            Meet.RemoveGymnast(tg3);

            Meet.RemoveDivision(testDivision);

            Assert.AreEqual<string>(testMeetFirstRoster, Meet.RosterString(),
                "First difference at index: " + HelperMethods.findDifference(Meet.RosterString(), testMeetFirstRoster).Item1
                + Environment.NewLine + HelperMethods.findDifference(Meet.RosterString(), testMeetFirstRoster).Item2);

        }

        [TestMethod]
        public void TestMethod012() {
            string exceptionMessage = ProgramConstants.NO_EXCEPTION_MSG;

            Assert.AreEqual<double?>(new double?(), margaretThatcher.vaultScore.score, "a");

            try { Assert.AreEqual<bool>(true, (margaretThatcher.vaultScore.score == null), "b"); }
            catch (Exception e) { exceptionMessage = e.Message; }

            Assert.AreEqual<string>(ProgramConstants.NO_EXCEPTION_MSG, exceptionMessage, "c");
        }

        [TestMethod]
        public void TestMethod013() {
            Assert.AreEqual<string>(testMeetBarsStandings, Meet.createString_gymnastStandings("Bars"));
        }

        [TestMethod]
        public void TestMethod014() {
            Assert.AreEqual<string>(testMeetBeamStandings, Meet.createString_gymnastStandings("Beam"));
        }

        [TestMethod]
        public void TestMethod015() {
            Assert.AreEqual<string>(testMeetFloorStandings, Meet.createString_gymnastStandings("Floor"));
        }

        [TestMethod]
        public void TestMethod016() {
            Assert.AreEqual<string>(testMeetVaultStandings, Meet.createString_gymnastStandings("Vault"));
        }

        [TestMethod]
        public void TestMethod017() {
            Assert.AreEqual<string>(testMeetOverallStandings, Meet.createString_gymnastStandings("All Around"));
        }

        [TestMethod]
        public void TestMethod018() {
            Assert.AreEqual<string>(testMeetOverallStandings, Meet.createString_gymnastStandings("All-Around"));
        }

        [TestMethod]
        public void TestMethod019() {
            Assert.AreEqual<string>(testMeetOverallStandings, Meet.createString_gymnastStandings("Overall"));
        }

        [TestMethod]
        public void TestMethod020() {
            string exceptionMessage = "";
            try { Meet.createString_gymnastStandings("Individual"); }
            catch (Exception e) { exceptionMessage = e.Message; }
            Assert.AreEqual<string>("EXCEPTION: unknown event INDIVIDUAL", exceptionMessage);
        }
        #endregion Testing_OrderingFiltering.

        [TestMethod]
        public void TestMethod021() {

            Division testDivision = new Division("Test");
            Meet.AddDivision(testDivision);
            Gymnast wendyWhiplash = new Gymnast("Wendy", "Whiplash", imaginaryWonders, "Test");
            Meet.AddGymnast(wendyWhiplash);
            Gymnast laurieHernandez = new Gymnast("Laurie", "Hernandez", olympicTypes, "Test");
            Meet.AddGymnast(laurieHernandez);
            Gymnast madisonKocian = new Gymnast("Madison", "Kocian", olympicTypes, "Test");
            Meet.AddGymnast(madisonKocian);

            Assert.AreEqual<string>(testMeetUpdatedRoster, Meet.RosterString());
            Assert.AreEqual<string>(testMeetFirstRoster, Meet.RosterString("Open"));
            Assert.AreEqual<string>(testMeetUpdatedTestDivisionRoster, Meet.RosterString("Test"));

            wendyWhiplash.floorScore = new EventScore(1.775);
            wendyWhiplash.barsScore = new EventScore(2.775);
            wendyWhiplash.beamScore = new EventScore(3.775);

            laurieHernandez.floorScore = new EventScore(9.4);
            laurieHernandez.barsScore = new EventScore(9.3);
            laurieHernandez.beamScore = new EventScore(9.2);
            laurieHernandez.vaultScore = new EventScore(9.1);

            madisonKocian.barsScore = new EventScore(9.9);
            madisonKocian.beamScore = new EventScore(9);
            madisonKocian.vaultScore = new EventScore(9);

            Assert.AreEqual<string>(testMeetUpdatedBarsStandings, Meet.createString_gymnastStandings("bars"), "bars");
            Assert.AreEqual<string>(testMeetUpdatedBeamStandings, Meet.createString_gymnastStandings("beam"), "beam");
            Assert.AreEqual<string>(testMeetUpdatedFloorStandings, Meet.createString_gymnastStandings("floor"), "floor");
            Assert.AreEqual<string>(testMeetUpdatedVaultStandings, Meet.createString_gymnastStandings("vault"), "vault");
            Assert.AreEqual<string>(testMeetUpdatedOverallStandings, Meet.createString_gymnastStandings("all around"), "all around");

            Assert.AreEqual<string>(testMeetUpdatedBarsStandings_TestDivision, Meet.createString_gymnastStandings("bars", testDivision), "bars test div");
            Assert.AreEqual<string>(testMeetUpdatedBeamStandings_TestDivision, Meet.createString_gymnastStandings("beam", testDivision), "beam  test div");
            Assert.AreEqual<string>(testMeetUpdatedFloorStandings_TestDivision, Meet.createString_gymnastStandings("floor", testDivision), "floor test div");
            Assert.AreEqual<string>(testMeetUpdatedVaultStandings_TestDivision, Meet.createString_gymnastStandings("vault", testDivision), "vault test div");
            Assert.AreEqual<string>(testMeetUpdatedOverallStandings_TestDivision, Meet.createString_gymnastStandings("all around", testDivision), "all around test div");
        }

        // TESTING FOR COPY CONSTRUCTORS (central to serialization and restoration approach)
        [TestMethod]
        public void TestMethod100() {

            Division d1 = new Division("d1");
            Division d1a = new Division(d1);

            Assert.AreEqual<bool>(true, d1.name == d1a.name);

            d1 = new Division("not d1");
            Assert.AreEqual<bool>(false, d1.name == d1a.name);
        }

        [TestMethod]
        public void TestMethod101() {
            Team t1 = new Team("Mod Squad");
            Team t2 = new Team(t1);

            Assert.AreEqual<string>(t1.name, t2.name);
            Assert.AreEqual<int>(t1.number, t2.number);

            Assert.AreEqual<bool>(true, t1.name == t2.name);
            Assert.AreEqual<bool>(true, t1.number == t2.number);

            t1 = new Team("Charlie's Angels");

            Assert.AreEqual<bool>(false, t1.name == t2.name);
        }

        [TestMethod]
        public void TestMethod102() {

            Gymnast testGymnast = new Gymnast("Test", "Figure", imaginaryWonders, "Open");
            testGymnast.vaultScore = new EventScore(5.1);
            testGymnast.barsScore = new EventScore(5.2);
            testGymnast.beamScore = new EventScore(5.3);
            testGymnast.floorScore = new EventScore(5.4);

            Gymnast copyGymnast = new Gymnast(testGymnast);

            Assert.AreEqual<string>(testGymnast.firstName, copyGymnast.firstName);
            Assert.AreEqual<string>(testGymnast.lastName, copyGymnast.lastName);
            Assert.AreEqual<string>(testGymnast.team.name, copyGymnast.team.name);
            Assert.AreEqual<string>(testGymnast.division.name, copyGymnast.division.name);
            Assert.AreEqual<double?>(testGymnast.vaultScore.score, copyGymnast.vaultScore.score);
            Assert.AreEqual<double?>(testGymnast.barsScore.score, copyGymnast.barsScore.score);
            Assert.AreEqual<double?>(testGymnast.beamScore.score, copyGymnast.beamScore.score);
            Assert.AreEqual<double?>(testGymnast.floorScore.score, copyGymnast.floorScore.score);
            Assert.AreEqual<double?>(testGymnast.overallScore, copyGymnast.overallScore);

            Assert.AreEqual<bool>(true, testGymnast.vaultScore == copyGymnast.vaultScore);
            Assert.AreEqual<bool>(true, testGymnast.beamScore == copyGymnast.beamScore);
            Assert.AreEqual<bool>(true, testGymnast.overallScore == copyGymnast.overallScore);
            testGymnast.vaultScore = new EventScore(4.1);
            Assert.AreEqual<bool>(false, testGymnast.vaultScore == copyGymnast.vaultScore);
            Assert.AreEqual<bool>(true, testGymnast.beamScore == copyGymnast.beamScore);
            Assert.AreEqual<bool>(false, testGymnast.overallScore == copyGymnast.overallScore);

        }


        // TESTING SERIALIZATION and DESERIALIZATION
        #region Testing_Serialization.
        [TestMethod]
        public async Task TestMethod110() {
            List<Gymnast> gymnastsToSerialize = new List<Gymnast>();

            gymnastsToSerialize.Add(stephanieGundel);
            gymnastsToSerialize.Add(lynneaCommon);
            gymnastsToSerialize.Add(annaUngar);
            gymnastsToSerialize.Add(maryLouRetton);

            string exceptionMessage = ProgramConstants.NO_EXCEPTION_MSG;

            try { await FileManagement.Serialize<List<Gymnast>>(gymnastsToSerialize, "Text File", ".txt", @"defaultSerializationFile.txt", @"Serialization"); }
            catch (Exception e) { exceptionMessage = e.Message + " exception of type: " + e.GetType().Name; }

            Assert.AreEqual<string>(ProgramConstants.NO_EXCEPTION_MSG, exceptionMessage);
        }

        [TestMethod]
        public async Task TestMethod111() {
            List<Gymnast> gymnastsToRestore = new List<Gymnast>();

            gymnastsToRestore.Add(stephanieGundel);
            gymnastsToRestore.Add(lynneaCommon);
            gymnastsToRestore.Add(annaUngar);
            gymnastsToRestore.Add(maryLouRetton);

            string exceptionMessage = ProgramConstants.NO_EXCEPTION_MSG;

            List<Gymnast> deserializedGymnasts = null; 
            try {  deserializedGymnasts = await FileManagement.Recover<List<Gymnast>>(".txt", @"defaultSerializationFile.txt", @"Serialization"); }
            catch(Exception e) { exceptionMessage = e.Message + " exception of type: " + e.GetType().Name; }

            Assert.AreEqual<string>(ProgramConstants.NO_EXCEPTION_MSG, exceptionMessage);

            int i = 0;
            foreach(Gymnast expected in gymnastsToRestore) {
                Assert.AreEqual<string>(expected.ToString(), deserializedGymnasts[i].ToString());
                ++i;
            }
        }

        [TestMethod]
        public async Task TestMethod112() {
            string exceptionMessage = ProgramConstants.NO_EXCEPTION_MSG;

            try { await Meet.sendForSerialization(@"msarcSerializationFile.txt", @"Serialization"); }
            catch (Exception e) { exceptionMessage = e.Message + " exception of type: " + e.GetType().Name; }

            Assert.AreEqual<string>(ProgramConstants.NO_EXCEPTION_MSG, exceptionMessage);

        }

        [TestMethod]
        public async Task TestMethod113() {
            Assert.AreEqual<int>(23, Meet.GymnastCount, "a");

            Gymnast shouldNotBeInRestore01 = new Gymnast("Casper", "the Ghost", imaginaryWonders, "Open");
            Meet.AddGymnast(shouldNotBeInRestore01);
            Gymnast shouldNotBeInRestore02 = new Gymnast("Scooby", "Doo", imaginaryWonders, "Open");
            Meet.AddGymnast(shouldNotBeInRestore02);

            Meet.RemoveGymnast(simoneBiles);
            Meet.RemoveGymnast(alyRaisman);
            Meet.RemoveGymnast(stephanieGundel);

            Assert.AreEqual<int>(22, Meet.GymnastCount, "b");

            string exceptionMessage = ProgramConstants.NO_EXCEPTION_MSG;

            try { await Meet.restoreFromSerialization(@"msarcSerializationFile.txt", @"Serialization"); }
            catch (Exception e) { exceptionMessage = e.Message + " exception of type: " + e.GetType().Name; }

            Assert.AreEqual<string>(ProgramConstants.NO_EXCEPTION_MSG, exceptionMessage);
            Assert.AreEqual<int>(23, Meet.GymnastCount, "c");
        }


        #endregion Testing_Serialization.
    }
}
