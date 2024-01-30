﻿namespace DndOnline.Models;

public class FileModel
{
    public Guid Id { get; set; }
    public string Caption { get; set; }
    public string Path { get; set; }
    public string Extension { get; set; }
    public long Size { get; set; }
}