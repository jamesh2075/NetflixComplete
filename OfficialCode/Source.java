package com.netflix.mediaclient.service.player.nrdpplayback.logblob;

import org.json.JSONObject;

public class LogReposition
  extends BaseStreamLogblob
{
  public LogReposition(String paramString, long paramLong1, long paramLong2, long paramLong3, long paramLong4)
  {
    super(paramString);
    this.mJson.put("soffms", paramLong1);
    this.mJson.put("moff", paramLong2 / 1000L);
    this.mJson.put("moffms", paramLong2);
    this.mJson.put("moffold", paramLong3 / 1000L);
    this.mJson.put("moffoldms", paramLong3);
    this.mJson.put("navt", paramLong4);
  }
  
  public String getType()
  {
    return "repos";
  }
}
