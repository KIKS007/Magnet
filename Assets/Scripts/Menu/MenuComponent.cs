using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuComponent : MonoBehaviour 
{
	public MenuComponentType menuComponentType;

	public RectTransform content = null;

	public List<RectTransform> underMenuList;
}
