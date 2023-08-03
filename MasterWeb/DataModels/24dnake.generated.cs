// Decompiled with JetBrains decompiler
// Type: WebHome.DataModels.talk_logger
// Assembly: MessageCenter, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F7B9E225-6A49-454A-B092-9C198D8EF217
// Assembly location: G:\temp\MessageCenter.dll

using LinqToDB.Mapping;
using System;

namespace WebHome.DataModels
{
  [Table("talk_logger")]
  public class talk_logger
  {
    [PrimaryKey]
    [Identity]
    public int id { get; set; }

    [Column]
    [NotNull]
    public string user { get; set; }

    [Column]
    [NotNull]
    public string to_user { get; set; }

    [Column]
    [Nullable]
    public int? mode { get; set; }

    [Column]
    [Nullable]
    public DateTime? ts { get; set; }
  }
}
