using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ArthemyDevelopment.Localization
{
	public class ExampleCustomComponent : MonoBehaviour
	{
		public List<string> ExampleStringList;

		private void OnDisable()
		{
			ExampleStringList.Clear();
		}

		public void AddStringToList(string s)
		{			
			ExampleStringList.Add(s);
		}

}
}
