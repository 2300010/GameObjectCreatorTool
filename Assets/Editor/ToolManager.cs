using UnityEngine;

public delegate TypeOfObject TypeOfObjectHasChanged(TypeOfObject selectedType);
public delegate PrimitiveType SelectedPrimitiveTypeChanged(PrimitiveType selectedPrimitiveType);
public delegate GameObject SelectedPrefabChanged(GameObject selectedPrefab);

public class ToolManager : ScriptableObject
{
    public TypeOfObjectHasChanged OnTypeOfObjectChange;
    public SelectedPrimitiveTypeChanged OnSelectedPrimitiveTypeChanged;
    public SelectedPrefabChanged OnSelectedPrefabChanged;

    private TypeOfObject selectedTypeOfObject;
    private PrimitiveType selectedPrimitiveType;
    private GameObject selectedPrefab;

    public TypeOfObject SelectedTypeOfObject
    {
        get => selectedTypeOfObject;
        set
        {
            if (selectedTypeOfObject != value)
            {
                selectedTypeOfObject = value;
                OnTypeOfObjectChange?.Invoke(selectedTypeOfObject);
            }
        }
    }
    public PrimitiveType SelectedPrimitiveType
    {
        get => selectedPrimitiveType;
        set
        {
            if (selectedPrimitiveType != value)
            {
                selectedPrimitiveType = value;
                OnSelectedPrimitiveTypeChanged?.Invoke(selectedPrimitiveType);
            }
        }
    }
    public GameObject SelectedPrefab
    {
        get => selectedPrefab;
        set
        {
            if (selectedPrefab != value)
            {
                selectedPrefab = value;
                OnSelectedPrefabChanged?.Invoke(selectedPrefab);
            }
        }
    }
}
