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
        /// Add the new DroneCharge to DroneCharges.
        /// </summary>
        /// <param name="newDroneCharge">DroneCharge to add.</param>
        public void AddDroneToCharge(DroneCharge newDroneCharge)
        {
            XElement droneChargeRoot = XMLTools.LoadData(dir + droneChargeFilePath);
            droneChargeRoot.Add(newDroneCharge.ToXElement<DroneCharge>());
            droneChargeRoot.Save(dir + droneChargeFilePath);

            #region LoadListFromXMLSerializer
            //IEnumerable<DO.DroneCharge> droneChargesList = XMLTools.LoadListFromXMLSerializer<DO.DroneCharge>(dir + droneChargeFilePath);
            //List<DroneCharge> newList = new List<DroneCharge>();
            //if (droneChargesList.Count() != 0)
            //{
            //    newList = droneChargesList.Cast<DO.DroneCharge>().ToList();
            //}
            //newList.Add(newDroneCharge);
            //XMLTools.SaveListToXMLSerializer<DO.DroneCharge>(newList, dir + droneChargeFilePath);
            #endregion
        }

        /// <summary>
        /// Remove charging drone by drone id.
        /// <param name="droneId">The charging drone with droneId - to remove</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void removeDroneChargeByDroneId(int droneId)
        {
            try
            {
                List<DO.DroneCharge> dronesList = XMLTools.LoadListFromXMLSerializer<DO.DroneCharge>(dir + droneChargeFilePath).ToList();
                int index = dronesList.FindIndex(d => d.DroneId == droneId);
                dronesList.RemoveAt(index);
                XMLTools.SaveListToXMLSerializer<DO.DroneCharge>(dronesList, dir + droneChargeFilePath);
            }
            catch (Exception e1)
            {
                throw new Exceptions.ObjNotExistException(typeof(Drone), droneId, e1);
            }
        }

        /// <summary>
        /// Get a DroneCharge/s with a specific condition = predicate
        /// </summary>
        /// <param name="predicate">return a drone charge /s that meeets the condition</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<DroneCharge> getDroneChargeWithSpecificCondition(Predicate<DroneCharge> predicate)
        {
            try
            {
                IEnumerable<DO.DroneCharge> droneChargeList = XMLTools.LoadListFromXMLSerializer<DO.DroneCharge>(dir + droneChargeFilePath);
                return (from droneCharge in droneChargeList
                        where predicate(droneCharge)
                        select droneCharge);
            }
            catch (Exception e)
            {
                throw new Exceptions.ObjNotExistException($"{e.Message}");
            }

            #region LoadData
            //XElement droneChargeRoot = XMLTools.LoadData(dir + droneChargeFilePath);
            //return (from d in droneChargeRoot.Elements()
            //        where predicate(returnDroneCharge(d))
            //        select returnDroneCharge(d));
            #endregion
        }

    }
}