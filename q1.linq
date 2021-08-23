from s in db.tblSheets
from rtp in db.tblResourceTypeParameters
where
  s.tblResourcetp.tblResourceType.resourceTypeName == "Ponding Points"
select new {
  Type_ = s.tblResourcetp.tblResourceType.resourceTypeName,
  category = s.tblResource.resourceName,
  townName = "Lahore",
  townName2 = s.tblResource.resourceName,
  ponding = s.parameterValue,
  pName = rtp.tblParameter.parameterName,
  unit = rtp.tblParameter.parameterUnit,
  lat = s.tblResource.resourceGeoLocatin.Substring(1 -1 , (SqlFunctions.CharIndex(",",s.tblResource.resourceGeoLocatin) == 0 ? (System.Int64)(int)s.tblResource.resourceGeoLocatin.Length : (SqlFunctions.CharIndex(",",s.tblResource.resourceGeoLocatin) - 1))),
  lng = s.tblResource.resourceGeoLocatin.Substring((SqlFunctions.CharIndex(",",s.tblResource.resourceGeoLocatin) == 0 ? ((int)s.tblResource.resourceGeoLocatin.Length + 1) : (SqlFunctions.CharIndex(",",s.tblResource.resourceGeoLocatin) + 1)) -1 , 1000),
  s.sheetInsertionDateTime,
  DeltaMinutes = (int?)SqlFunctions.DateDiff("minute",s.sheetInsertionDateTime,SqlFunctions.GetDate())
}