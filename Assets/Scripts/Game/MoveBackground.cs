using UnityEngine;

public class MoveBackground : MonoBehaviour
{
    [SerializeField] private MeshRenderer _bgRender;    // рендер отображающий текстуру
    [SerializeField] private float _speed = 0.4f;      // скорость вращения текстуры

    private Vector2 _startOffset;   // смещение
    private int _mainTextureId = Shader.PropertyToID("_MainTex"); // ID св-во main текстур
    private float _tempYOffset;
    
    // Start is called before the first frame update
    void Start()
    {
        _startOffset = _bgRender.sharedMaterial.GetTextureOffset(_mainTextureId);
    }

    // Update is called once per frame
    void Update()
    {
        _tempYOffset = Mathf.Repeat(_tempYOffset + Time.deltaTime * _speed, 1);
        Vector2 offset = new Vector2(_startOffset.x, _tempYOffset);
        _bgRender.sharedMaterial.SetTextureOffset(_mainTextureId, offset);
    }
}
