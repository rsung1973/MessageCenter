// Decompiled with JetBrains decompiler
// Type: WebHome.DataModels.device
// Assembly: MessageCenter, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F7B9E225-6A49-454A-B092-9C198D8EF217
// Assembly location: G:\temp\MessageCenter.dll

using LinqToDB.Mapping;
using System;

namespace WebHome.DataModels
{
  [Table("devices")]
  public class device
  {
    [PrimaryKey]
    [Identity]
    public int id { get; set; }

    [Column]
    [NotNull]
    public string user { get; set; }

    [Column]
    [Nullable]
    public int? idx { get; set; }

    [Column]
    [Nullable]
    public string url { get; set; }

    [Column]
    [Nullable]
    public string proxy_url { get; set; }

    [Column]
    [Nullable]
    public int? proxy_ok { get; set; }

    [Column]
    [Nullable]
    public DateTime? heartbeat { get; set; }
  }
}
