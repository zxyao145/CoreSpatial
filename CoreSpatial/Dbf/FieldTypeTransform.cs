using System;

namespace CoreSpatial.Dbf
{
    internal class FieldTypeTransform
    {
        public static Type DbfFieldType2CSharpType(DbfFieldInfo dbfFieldInfo)
        {
            Type cSharpType;
            switch (dbfFieldInfo.FieldType)
            {
                case DbfFieldType.Character:
                    cSharpType = typeof(string);
                    break;

               case DbfFieldType.Numeric:
                    if (dbfFieldInfo.Accuracy == 0)
                    {
                        if (dbfFieldInfo.FieldLength == 4)
                        {
                            cSharpType = typeof(int);
                        }
                        else if(dbfFieldInfo.FieldLength <=8)
                        {
                            cSharpType = typeof(long);
                        }
                        else
                        {
                            cSharpType = typeof(double);
                        }
                    }
                    else
                    {
                        cSharpType = dbfFieldInfo.Accuracy > 6 ? typeof(double) : typeof(float);
                    }
                    break;
                case DbfFieldType.Date:
                    cSharpType = typeof(DateTime);
                    break;

                case DbfFieldType.Logical:
                    cSharpType = typeof(bool);
                    break;

                case DbfFieldType.General:
                case DbfFieldType.Byte:
                default:
                    cSharpType = typeof(byte[]);
                    break;
            }
            return cSharpType;
        }

        public static DbfFieldType CSharpType2DbfFieldType(Type t)
        {
            DbfFieldType dbfFieldType;
            switch (t.Name)
            {
                case "Int32":
                case "Int64":
                case "Double":
                case "Single"://N
                    dbfFieldType = DbfFieldType.Numeric;
                    break;
                case "String"://C
                    dbfFieldType = DbfFieldType.Character;
                    break;
                case "DateTime"://D
                    dbfFieldType = DbfFieldType.Date;
                    break;
                case "Boolean"://L
                    dbfFieldType = DbfFieldType.Logical;
                    break;
                default://G
                    dbfFieldType = DbfFieldType.General;
                    break;
            }
            return dbfFieldType;
        }

        public static DbfFieldInfo
            CSharpType2DbfFieldInfoWithoutName(Type t)
        {
            DbfFieldType dbfFieldType;
            byte fieldLength = 0;
            byte accuracy = 0;

            switch (t.Name)
            {
                case "Byte":
                    fieldLength = 1;
                    dbfFieldType = DbfFieldType.Numeric;
                    accuracy = 0;
                    break;
                case "Int16":
                    fieldLength = 2;
                    dbfFieldType = DbfFieldType.Numeric;
                    accuracy = 0;
                    break;
                case "Int32":
                    fieldLength = 4;
                    dbfFieldType = DbfFieldType.Numeric;
                    accuracy = 0;
                    break;
                case "Int64":
                    fieldLength = 8;
                    dbfFieldType = DbfFieldType.Numeric;
                    accuracy = 0;
                    break;
                case "Double":
                    fieldLength = 31;
                    dbfFieldType = DbfFieldType.Numeric;
                    accuracy = 15;
                    break;
                case "Single"://N
                    fieldLength = 15;
                    dbfFieldType = DbfFieldType.Numeric;
                    accuracy = 6;
                    break;
                case "String"://C
                    dbfFieldType = DbfFieldType.Character;
                    fieldLength = 255;
                    break;
                case "DateTime"://D
                    dbfFieldType = DbfFieldType.Date;
                    fieldLength = 8;
                    break;
                case "Boolean"://L
                    dbfFieldType = DbfFieldType.Logical;
                    fieldLength = 1;
                    break;
                default://G
                    dbfFieldType = DbfFieldType.General;
                    //TODO 优化
                    fieldLength = 255;
                    break;
            }

            return new DbfFieldInfo()
            {
                Accuracy = accuracy,
                FieldLength = fieldLength,
                FieldType = dbfFieldType
            };
        }
    }
}
