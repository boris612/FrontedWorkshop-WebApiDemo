﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

#nullable disable

namespace Backend.DAL.Models
{
    public partial class School
    {
        public School()
        {
            Student = new HashSet<Student>();
            Workshop = new HashSet<Workshop>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int TownId { get; set; }

        public virtual Town Town { get; set; }
        public virtual ICollection<Student> Student { get; set; }
        public virtual ICollection<Workshop> Workshop { get; set; }
    }
}