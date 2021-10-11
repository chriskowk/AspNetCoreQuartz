using System;
using System.Collections.Generic;
using System.Text;

namespace AsyncBreakfast
{
    public abstract class Food
    {
        public abstract int Id { get; }
        public abstract string Name { get; }
    }

    public class Toast : Food
    {
        public override int Id => 1;
        public override string Name => "Toast";
    }
    
    public class Juice : Food
    {
        public override int Id => 2;
        public override string Name => "Juice";
    }

    public class Bacon : Food
    {
        public override int Id => 3;
        public override string Name => "Bacon";
    }

    public class Egg : Food
    {
        public override int Id => 4;
        public override string Name => "Egg";
    }

    public class Coffee : Food
    {
        public override int Id => 5;
        public override string Name => "Coffee";
    }
}
