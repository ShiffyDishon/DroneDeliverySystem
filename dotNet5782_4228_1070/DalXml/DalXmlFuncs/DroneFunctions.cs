using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using DO;
using System.Runtime.CompilerServices;



namespace Dal
{
    public sealed partial class DalXml : DalApi.IDal
    {

        /// <summary>
        /// Add the new drone to Drones.
        /// </summary>
        /// <param name="newDrone">drone to add.</param>
        public void AddDrone(DO.Drone newDrone)
        {
            Drone drone;
            try
            {
                drone = getDroneWithSpecificCondition(d => d.Id == newDrone.Id).First();
                if (drone.IsActive)
                    throw new Exceptions.ObjExistException(typeof(Drone), newDrone.Id);

                changeDroneInfo(newDrone);
                throw new Exceptions.DataChanged(typeof(Drone), newDrone.Id);

            }
            catch (Exception)
            {
                XElement DroneRoot = XMLTools.LoadData(dir + droneFilePath);
                DroneRoot.Add(newDrone.ToXElement<Drone>());
                DroneRoot.Save(dir + droneFilePath);
            }

            #region LoadListFromXMLSerializer
            //Drone drone;
            //try
            //{
            //    drone = getDroneWithSpecificCondition(d => d.Id == newDrone.Id).First();
            //    if (drone.IsActive)
            //        throw new Exceptions.ObjExistException(typeof(Drone), newDrone.Id);

            //    changeDroneInfo(newDrone);
            //    throw new Exceptions.DataChanged(typeof(Drone), newDrone.Id);
            //}
            //catch (Exception)
            //{
            //  IEnumerable<DO.Drone> droneList = XMLTools.LoadListFromXMLSerializer<DO.Drone>(dir + droneFilePath);
            //  droneList.ToList().Add(newDrone);
            //  DL.XMLTools.SaveListToXMLSerializer<DO.Drone>(droneList, dir + droneFilePath);            //    throw new Exceptions.ObjNotExistException(typeof(Drone), newDrone.Id);
            //}
            #endregion
        }

        /// <summary>
        /// if drone exist: IsActive = false + change its info (In DataSource)
        /// If doesn't exist throw NoMatchingData exception.</summary>
        /// <param name="droneToRemove">The drone to remove. droneToRemove.IsActive = false</param>
        public void removeDrone(Drone droneToRemove)
        {
            try
            {
                XElement droneRoot = XMLTools.LoadData(dir + droneFilePath);
                XElement droneXElemnt = (from d in droneRoot.Elements()
                                         where Convert.ToInt32(d.Element("Id").Value) == droneToRemove.Id
                                         select d).FirstOrDefault();

                if (droneXElemnt != null)
                    droneXElemnt.Element("IsActive").Value = "false";
            }
            catch (Exception)
            {
                throw new Exceptions.ObjNotExistException(typeof(Drone), droneToRemove.Id);
            }

            #region LoadListFromXMLSerializer
            //IEnumerable<DO.Drone> dronesList = XMLTools.LoadListFromXMLSerializer<DO.Drone>(dir + droneFilePath);
            //try
            //{
            //    Drone drone = getDroneWithSpecificCondition(d => d.Id == droneToRemove.Id).First();
            //    if (drone.IsActive)
            //        drone.IsActive = false;
            //    changeDroneInfo(drone);
            //}
            //catch (Exception e1)
            //{
            //    throw new Exceptions.NoMatchingData(typeof(Drone), droneToRemove.Id, e1);
            //}


            //public void removeDrone(int index)
            //{
            //    XElement droneRoot = XMLTools.LoadData(dir + droneFilePath);
            //    int i = 0;
            //    XElement droneXElemnt = (from d in droneRoot.Elements()
            //                             where i++ == index
            //                             select d).FirstOrDefault();

            //    if (droneXElemnt != null)
            //        droneXElemnt.Element("IsActive").Value = "false";

            //    #region LoadListFromXMLSerializer
            //    //IEnumerable<DO.Drone> dronesList = XMLTools.LoadListFromXMLSerializer<DO.Drone>(dir + droneFilePath);
            //    //try
            //    //{
            //    //    Drone drone = getDroneWithSpecificCondition(d => d.Id == droneToRemove.Id).First();
            //    //    if (drone.IsActive)
            //    //        drone.IsActive = false;
            //    //    changeDroneInfo(drone);
            //    //}
            //    //catch (Exception e1)
            //    //{
            //    //    throw new Exceptions.NoMatchingData(typeof(Drone), droneToRemove.Id, e1);
            //    //}
            //    #endregion
            //}
            #endregion
        }

        /// <summary>
        /// Get all drone.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Drone> GetDrones()
        {
            XElement droneRoot = XMLTools.LoadData(dir + droneFilePath);

            return (from d in droneRoot.Elements()
                    orderby Convert.ToInt32(d.Element("Id").Value)
                    select d.FromXElement<Drone>());

            #region found a better way
            //IEnumerable<DO.Drone> dronesList = XMLTools.LoadListFromXMLSerializer<DO.Drone>(dir + droneFilePath);
            //return from item in dronesList
            //       orderby item.Id
            //       select item;
            #endregion
        }

        /// <summary>
        /// Change specific drone info
        /// </summary>
        /// <param name="droneWithUpdateInfo">Drone with the changed info</param>
        public void changeDroneInfo(Drone droneWithUpdateInfo)
        {
            XElement droneRoot = XMLTools.LoadData(dir + droneFilePath);
            XElement droneElement = (from p in droneRoot.Elements()
                                    where Convert.ToInt32(p.Element("Id").Value) == droneWithUpdateInfo.Id
                                    select p).FirstOrDefault();

            XElement xElementUpdateDrone = droneWithUpdateInfo.ToXElement<Drone>();
            droneElement.ReplaceWith(xElementUpdateDrone);
            droneRoot.Save(dir + droneFilePath);

            #region LoadListFromXMLSerializer
            //List<DO.Drone> droneList = XMLTools.LoadListFromXMLSerializer<DO.Drone>(dir + droneFilePath).ToList();
            //int index = droneList.FindIndex(t => t.Id == droneWithUpdateInfo.Id);
            //if (index == -1)
            //    throw new DO.Exceptions.ObjNotExistException(typeof(Drone), droneWithUpdateInfo.Id);

            //droneList[index] = droneWithUpdateInfo;
            //XMLTools.SaveListToXMLSerializer<DO.Drone>(droneList, dir + droneFilePath);
            #endregion
        }

        /// <summary>
        /// Get a Drone/s with a specific condition = predicate
        /// </summary>
        /// <param name="predicate">return a drone/s that meeets the condition</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Drone> getDroneWithSpecificCondition(Predicate<Drone> predicate)
        {
            XElement droneRoot = XMLTools.LoadData(dir + droneFilePath);
            return (from d in droneRoot.Elements()
                    where predicate(d.FromXElement<Drone>())
                    select d.FromXElement<Drone>());

            #region LoadListFromXMLSerializer
            //    IEnumerable<DO.Drone> droneList = XMLTools.LoadListFromXMLSerializer<DO.Drone>(dir + droneFilePath);
            //    return (from drone in droneList
            //            where predicate(drone)
            //            select drone);
            #endregion
        }

        /// <summary>
        /// returns an array of drones' electricity usage. 
        /// arr[] =
        /// empty,
        /// lightWeight,
        /// mediumWeight,
        /// heavyWeight,
        /// chargingRate
        /// </summary>
        /// <returns></returns>
        public double[] electricityUseByDrone()
        {
            try
            {
                return XMLTools.LoadListFromXMLSerializer<double>(dir + configFilePath).ToArray();
            }
            catch (Exception e)
            {
                throw new Exceptions.FileLoadCreateException("Program cann't Load", $"{e.Message}", e);
            }
        }
    }
}