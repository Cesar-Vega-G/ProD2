using TMPro;
using TreeEditor;
using UnityEngine;
using UnityEngine.UI;

public class TreeDisplay : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private GameObject linePrefab;

    [Header("Settings")]
    [SerializeField] private float horizontalSpacing = 100f;
    [SerializeField] private float verticalSpacing = 80f;
    [SerializeField] private Color highlightColor = Color.yellow;

    private RectTransform container;
    private int playerId;

    private void Awake()
    {
        container = GetComponent<RectTransform>();
        playerId = transform.name.Contains("1") ? 0 : 1; // Detecta Player1 o Player2
    }

    private void Start()
    {
        TreeManager.Instance.OnTreeUpdated += UpdateTreeView;
    }

    public void UpdateTreeView(int updatedPlayerId, ITree tree)
    {
        if (updatedPlayerId != playerId) return;

        ClearTree();
        if (tree?.Root != null)
        {
            DrawNode(tree.Root, container.rect.width / 2, container.rect.height - 50f);
        }
    }

    private void DrawNode(ITreeNode node, float x, float y)
    {
        GameObject nodeObj = Instantiate(nodePrefab, transform);
        nodeObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
        nodeObj.GetComponentInChildren<TextMeshProUGUI>().text = node.Value.ToString();

        if (IsRelevantForChallenge(node))
        {
            nodeObj.GetComponent<Image>().color = highlightColor;
        }

        float childY = y - verticalSpacing;
        float childXOffset = horizontalSpacing * Mathf.Pow(0.5f, node.Depth + 1);

        if (node.Left != null)
        {
            float childX = x - childXOffset;
            DrawNode(node.Left, childX, childY);
            DrawConnection(x, y, childX, childY);
        }

        if (node.Right != null)
        {
            float childX = x + childXOffset;
            DrawNode(node.Right, childX, childY);
            DrawConnection(x, y, childX, childY);
        }
    }

    private void DrawConnection(float x1, float y1, float x2, float y2)
    {
        GameObject lineObj = Instantiate(linePrefab, transform);
        RectTransform lineRect = lineObj.GetComponent<RectTransform>();

        Vector2 midpoint = new Vector2((x1 + x2) / 2, (y1 + y2) / 2);
        float angle = Mathf.Atan2(y2 - y1, x2 - x1) * Mathf.Rad2Deg;
        float length = Vector2.Distance(new Vector2(x1, y1), new Vector2(x2, y2));

        lineRect.anchoredPosition = midpoint;
        lineRect.sizeDelta = new Vector2(length, 5f);
        lineRect.localRotation = Quaternion.Euler(0, 0, angle);
    }

    private bool IsRelevantForChallenge(ITreeNode node)
    {
        // Implementa lógica según tus retos
        return false;
    }

    private void ClearTree()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void OnDestroy()
    {
        if (TreeManager.Instance != null)
        {
            TreeManager.Instance.OnTreeUpdated -= UpdateTreeView;
        }
    }
}