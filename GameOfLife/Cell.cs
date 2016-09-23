namespace GameOfLife
{
    public class Cell
    {
        public bool IsAlive { get; set; }

        public bool HasBeenToggled { get; set; }

        public void ToggleStatus()
        {
            IsAlive = !IsAlive;
            HasBeenToggled = true;
        }
    }
}
