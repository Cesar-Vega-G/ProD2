public class AVLNode
{
    public int Value { get; set; }
    public AVLNode Left { get; set; }
    public AVLNode Right { get; set; }
    public int Height { get; set; }

    public AVLNode(int value)
    {
        Value = value;
        Height = 1;
    }
}