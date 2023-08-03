// Decompiled with JetBrains decompiler
// Type: WebHome.DataModels.routesavp
// Assembly: MessageCenter, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F7B9E225-6A49-454A-B092-9C198D8EF217
// Assembly location: G:\temp\MessageCenter.dll

using LinqToDB.Mapping;

namespace WebHome.DataModels
{
  [Table("routesavp")]
  public class routesavp
  {
    [PrimaryKey]
    [NotNull]
    public string attr { get; set; }

    [Column]
    [Nullable]
    public string value { get; set; }
  }
}
