using System;
using System.Collections.Generic;

public class Quote
{
    public int Id { get; set; }
    public string Author { get; set; }
    public string Text { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<string> Tags { get; set; }
    public Dictionary<string, object> Metadata { get; set; }
}

