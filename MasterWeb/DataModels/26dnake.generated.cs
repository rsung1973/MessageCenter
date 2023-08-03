// Decompiled with JetBrains decompiler
// Type: WebHome.DataModels.type
// Assembly: MessageCenter, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F7B9E225-6A49-454A-B092-9C198D8EF217
// Assembly location: G:\temp\MessageCenter.dll

using LinqToDB.Mapping;

namespace WebHome.DataModels
{
  [Table("types")]
  public class type
  {
    [PrimaryKey]
    [Identity]
    public int id { get; set; }

    [Column]
    [NotNull]
    public int tid { get; set; }

    [Column]
    [NotNull]
    public string name { get; set; }
  }
}
