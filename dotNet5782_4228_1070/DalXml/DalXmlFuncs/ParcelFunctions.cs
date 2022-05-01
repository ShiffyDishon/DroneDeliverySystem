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
        /// Add the new parcel to parcels.
        /// </summary>
        /// <param name="newParcel">parcel to add</param>
        public void AddParcel(Parcel newParcel)
        {
            XElement ParcelRoot = XMLTools.LoadData(dir + parcelFilePath);
            newParcel.IsActive = true;
            ParcelRoot.Add(newParcel.ToXElement<Parcel>());
            ParcelRoot.Save(dir + parcelFilePath);

            #region LoadListFromXMLSerializer 
            //Without the new chaeck if exist
            //List<DO.Parcel> parcelsList = XMLTools.LoadListFromXMLSerializer<DO.Parcel>(dir + parcelFilePath).ToList();
            //newParcel.IsActive = true;
            //parcelsList.Add(newParcel);
            //XMLTools.SaveListToXMLSerializer<DO.Parcel>(parcelsList, dir + parcelFilePath);
            #endregion
        }

        /// <summary>
        /// Remove specific parcel
        /// </summary>
        /// <param name="parcelToRemove">remove current parcel</param>
        public void removeParcel(Parcel parcelToRemove)
        {
            try
            {
                XElement parcelRoot = XMLTools.LoadData(dir + parcelFilePath);
                XElement parcelXElemnt = (from p in parcelRoot.Elements()
                                          where Convert.ToInt32(p.Element("Id").Value) == parcelToRemove.Id
                                          select p).FirstOrDefault();

                if (parcelXElemnt != null)
                    parcelXElemnt.Element("IsActive").Value = "false";
            }
            catch(Exception e1)
            {
                throw new Exceptions.ObjNotExistException(typeof(Parcel), parcelToRemove.Id, e1);
            }

            #region LoadListFromXMLSerializer
            //IEnumerable<DO.Parcel> parcelsList = XMLTools.LoadListFromXMLSerializer<DO.Parcel>(dir + parcelFilePath);
            //try
            //{
            //    Parcel parcel = getParcelWithSpecificCondition(s => s.Id == parcelToRemove.Id).First();
            //    if (parcel.IsActive)
            //        parcel.IsActive = false;
            //    changeParcelInfo(parcel);
            //}
            //catch (Exception e1)
            //{
            //    throw new Exceptions.NoMatchingData(typeof(Parcel), parcelToRemove.Id, e1);
            //}
            #endregion
        }

        /// <summary>
        /// Get all stations.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Parcel> GetParcels()
        {
            XElement parcelRoot = XMLTools.LoadData(dir + parcelFilePath);
            return (from p in parcelRoot.Elements()
                    orderby Convert.ToInt32(p.Element("Id").Value)
                    select p.FromXElement<Parcel>());//returnParcel(p));

            #region LoadListFromXMLSerializer
                    //IEnumerable<DO.Parcel> parceslList = XMLTools.LoadListFromXMLSerializer<DO.Parcel>(dir + parcelFilePath);
                    //return from item in parceslList
                    //       orderby item.Id
                    //       select item;
                    #endregion
        }

        /// <summary>
        ///  Change specific parcel info
        /// </summary>
        /// <param name="parcelWithUpdateInfo">The parcel with the changed info</param>
        public void changeParcelInfo(Parcel parcelWithUpdateInfo)
        {
            XElement ParcelRoot = XMLTools.LoadData(dir + parcelFilePath);
            XElement parcelElement = (from p in ParcelRoot.Elements()
                                      where Convert.ToInt32(p.Element("Id").Value) == parcelWithUpdateInfo.Id
                                      select p).FirstOrDefault();

            XElement xElementUpdateParcel = parcelWithUpdateInfo.ToXElement<Parcel>();
            parcelElement.ReplaceWith(xElementUpdateParcel);
            ParcelRoot.Save(dir + parcelFilePath);

            #region LoadListFromXMLSerializer
            //List<DO.Parcel> parcelsList = XMLTools.LoadListFromXMLSerializer<DO.Parcel>(dir + parcelFilePath).ToList();
            //int index = parcelsList.FindIndex(t => t.Id == parcelWithUpdateInfo.Id);
            //if (index == -1)
            //    throw new DO.Exceptions.ObjNotExistException(typeof(Parcel), parcelWithUpdateInfo.Id);

            //parcelsList[index] = parcelWithUpdateInfo;
            //XMLTools.SaveListToXMLSerializer<DO.Parcel>(parcelsList, dir + parcelFilePath);
            #endregion
        }

        /// <summary>
        /// Get a Parcel/s with a specific condition = predicate
        /// </summary>
        /// <param name="predicate">return a parcel/s that meeets the condition</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Parcel> getParcelWithSpecificCondition(Predicate<Parcel> predicate)
        {
            XElement parcelRoot = XMLTools.LoadData(dir + parcelFilePath);
            return (from p in parcelRoot.Elements()
                    where predicate(p.FromXElement<Parcel>())
                    select p.FromXElement<Parcel>());

            #region LoadListFromXMLSerializer
            //IEnumerable<DO.Parcel> parcelsList = XMLTools.LoadListFromXMLSerializer<DO.Parcel>(dir + parcelFilePath);
            //return (from parcel in parcelsList
            //        where predicate(parcel)
            //        select parcel);
            #endregion
        }



        /// <summary>
        /// Return how much parcels there is.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public int amountParcels()
        {
            try
            {
                return XMLTools.LoadListFromXMLSerializer<DO.Parcel>(dir + parcelFilePath).Count();
            }
            catch (Exception e)
            {
                throw new Exceptions.ObjNotExistException($"{e.Message}");
            }
        }
    }
}