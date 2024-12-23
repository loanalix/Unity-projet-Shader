using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    [SerializeField] float _destroyTime = 1f;
    [SerializeField] string _text;
    [SerializeField] Vector3 _offset = new Vector3(0,2,0);
    [SerializeField] Vector3 _randomOffset = new Vector3(0.5f,0,0);

    [SerializeField] List<Color> _colors;
    private int currentColorIndex = 0;
    private int targetColorIndex = 1;
    private float targetPoint;

    private TextMeshPro _textMesh;

    void Start()
    {
        if(_colors[0] == null)
        {
            _colors[0] = new Color();
            _colors[0] = Color.white;
        }

        _textMesh = GetComponent<TextMeshPro>();
        _textMesh.text = _text;
        _textMesh.color = _colors[0];

        transform.localPosition += _offset;
        transform.localPosition += new Vector3(Random.Range(-_randomOffset.x, _randomOffset.x), 
            Random.Range(-_randomOffset.y, _randomOffset.y), 
            Random.Range(-_randomOffset.z, _randomOffset.z));

        Destroy(gameObject, _destroyTime);
    }

    void Update()
    {
        // face camera
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);


        targetPoint += Time.deltaTime / (_destroyTime / (_colors.Count -1));

        _textMesh.color = Color.Lerp(_colors[currentColorIndex], _colors[targetColorIndex], targetPoint);
        if(targetPoint >= 1f)
        {
            targetPoint = 0f;
            currentColorIndex = targetColorIndex;
            targetColorIndex++;
            if(targetColorIndex == _colors.Count)
                targetColorIndex = 0;
        }
    }

    public void SetText(string text)
    {
        _text = text;
    }

    public void ClearColors()
    {
        _colors.Clear();
    }

    public void SetColor(Color color, int index = 0) {
        if (index >= _colors.Count) { 
            _colors.Add(color);
        } else
        {
            _colors[index] = color;
        }
    }
}
