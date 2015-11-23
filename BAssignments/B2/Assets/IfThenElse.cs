//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TreeSharpPlus
{
	public class IfThenElse : NodeGroup
	{
		private Node ifNode, thenNode, elseNode;
		public IfThenElse (Node ifNode, Node thenNode, Node elseNode)
		{
			this.ifNode = ifNode;
			this.thenNode = thenNode;
			this.elseNode = elseNode;
			ifNode.Parent = thenNode.Parent = elseNode.Parent = this;
		}

		public override IEnumerable<RunStatus> Execute()
		{
			Debug.Log ("IfThenElse start");
			ifNode.Start ();
			this.TickNode (ifNode);
			Debug.Log (ifNode.LastStatus);
			RunStatus executionStatus = RunStatus.Running;

			while (ifNode.LastStatus == RunStatus.Running) {
				Debug.Log ("If start");
				yield return RunStatus.Running;
				this.TickNode(ifNode);
			}

			if (ifNode.LastStatus == RunStatus.Success) {
				Debug.Log ("If success");
				thenNode.Start ();
				this.TickNode(thenNode);
				while (thenNode.LastStatus == RunStatus.Running) {
					yield return RunStatus.Running;
					this.TickNode(thenNode);
				}
			} else {
				Debug.Log ("If failure");
				elseNode.Start ();
				this.TickNode (elseNode);
				while (elseNode.LastStatus == RunStatus.Running) {
					yield return RunStatus.Running;
					this.TickNode(elseNode);
				}
			}
			yield return RunStatus.Success;
			yield break;
		}

	}
}
