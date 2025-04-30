public class BSTNode
{
    public int Value { get; set; }
    public BSTNode Left { get; set; }
    public BSTNode Right { get; set; }

    public BSTNode(int value)
    {
        Value = value;
        Left = Right = null;
    }
}
