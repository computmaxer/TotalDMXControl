using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Total_DMX_Control_WPF
{
    /*
     * This class holds information about all of the existing instances of Fixture and its subclasses.
     */
    static class Fixtures
    {
        #region Data Members
        /**
         * Lists/Hashtables of existing fixtures.
         * The Hashtable has every fixture in it.
         * The two lists are currently something specific to our setup, makes it easier to refference a group of lights.
         **/
        //private static List<Fixture> fixt = new List<Fixture>();
        //private static List<LedParFixture> led = new List<LedParFixture>();
        //private static List<DimmableFixture> pars = new List<DimmableFixture>();
        #endregion

        #region Properties
        //public static List<Fixture> List
        //{
        //    get { return fixt; }
        //}
        //public static List<LedParFixture> LEDs
        //{
        //    get { return led; }
        //}
        //public static List<DimmableFixture> PARs
        //{
        //    get { return pars; }
        //}
        #endregion

        /*
         * Acts sort of like a constructor. Gets called when the program starts.
         */
        public static void Start()
        {
            GenerateDefaultFixtures();
        }

        /*
         * This method will create the fixtures specific to our DJ setup.  This would not be called if someone else was using the software.
         */
        private static void GenerateDefaultFixtures()
        {
            #region PARs
            DimmableFixture par1 = new DimmableFixture("par1", 1);
            DimmableFixture par2 = new DimmableFixture("par2", 2);
            DimmableFixture par3 = new DimmableFixture("par3", 3);
            DimmableFixture par4 = new DimmableFixture("par4", 4);
            DimmableFixture par5 = new DimmableFixture("par5", 5);
            DimmableFixture par6 = new DimmableFixture("par6", 6);
            DimmableFixture par7 = new DimmableFixture("par7", 7);
            DimmableFixture par8 = new DimmableFixture("par8", 8);
            //Controller.Fixtures.Add(par1);
            //Controller.Fixtures.Add(par2);
            //Controller.Fixtures.Add(par3);
            //Controller.Fixtures.Add(par4);
            //Controller.Fixtures.Add(par5);
            //Controller.Fixtures.Add(par6);
            //Controller.Fixtures.Add(par7);
            //Controller.Fixtures.Add(par8);
            #endregion

            #region LEDs
            //LedParFixture led1 = new LedParFixture("led1", 17);
            //LedParFixture led2 = new LedParFixture("led2", 20);
            //LedParFixture led3 = new LedParFixture("led3", 23);
            //LedParFixture led4 = new LedParFixture("led4", 26);
            LedParFixture led1 = new LedParFixture("led1", 17);
            LedParFixture led2 = new LedParFixture("led2", 20);
            LedParFixture led3 = new LedParFixture("led3", 23);
            LedParFixture led4 = new LedParFixture("led4", 26);
            //Controller.Fixtures.Add(led1);
            //Controller.Fixtures.Add(led2);
            //Controller.Fixtures.Add(led3);
            //Controller.Fixtures.Add(led4);
            #endregion

            #region Others
            StrobeFixture strobe1 = new StrobeFixture("strobe1", 29);
            DimmableFixture mine1 = new DimmableFixture("mine1", 9);
            DimmableFixture mine2 = new DimmableFixture("mine2", 13);
            DimmableFixture rope1 = new DimmableFixture("rope1", 11);
            DimmableFixture rope2 = new DimmableFixture("rope2", 15);
            RevoXpressFixture revo = new RevoXpressFixture("revo", 31);
            DimmableFixture disco = new DimmableFixture("disco", 12);
            //Controller.Fixtures.Add(strobe1);
            //Controller.Fixtures.Add(mine1);
            //Controller.Fixtures.Add(mine2);
            //Controller.Fixtures.Add(rope1);
            //Controller.Fixtures.Add(rope2);
            //Controller.Fixtures.Add(revo);
            //Controller.Fixtures.Add(disco);
            Controller.Fixtures.Clear();
            Qspot260Fixture qspot = new Qspot260Fixture("qspot1", 51);
            Controller.Fixtures.Add(qspot);
            Qspot260Fixture qspot2 = new Qspot260Fixture("qspot2", 65);
            Controller.Fixtures.Add(qspot2);
            #endregion

            #region Add To Lists
            //pars.Add(par1);
            //pars.Add(par2);
            //pars.Add(par3);
            //pars.Add(par4);
            //pars.Add(par5);
            //pars.Add(par6);
            //pars.Add(par7);
            //pars.Add(par8);
            //led.Add(led1);
            //led.Add(led2);
            //led.Add(led3);
            //led.Add(led4);
            #endregion
        }

        ///**
        // * Add a fixture to the hashtable.
        // **/
        //public static void AddFixture(Fixture item)
        //{
        //    fixt.Add(item);
        //}

        ///**
        // * Remove a fixture with the given name from the hashtable.
        // **/
        //public static void RemoveFixture(Fixture item)
        //{
        //    fixt.Remove(item);
        //}
    }
}
