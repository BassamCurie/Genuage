﻿/**
Copyright (c) 2020, 	Institut Curie, Institut Pasteur and CNRS
			Thomas BLanc, Mohamed El Beheiry, Jean Baptiste Masson, Bassam Hajj and Clement Caporal
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
1. Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright
   notice, this list of conditions and the following disclaimer in the
   documentation and/or other materials provided with the distribution.
3. All advertising materials mentioning features or use of this software
   must display the following acknowledgement:
   This product includes software developed by the Institut Curie, Insitut Pasteur and CNRS.
4. Neither the name of the Institut Curie, Insitut Pasteur and CNRS nor the
   names of its contributors may be used to endorse or promote products
   derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDER ''AS IS'' AND ANY
EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL 
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE 
USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
**/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using VRTK.GrabAttachMechanics;
using Data;
using DesktopInterface;
/// <summary>
/// Box use to show to the user the collider box around the CloudPoints
/// </summary>
public class CloudBox : MonoBehaviour
{

    CloudData _cloud_status;
    GameObject _box;
    Material _material;
    bool _is_grabbed;

    int[] _lines = {
            0, 1,
            0, 3,
            0, 7,
            6, 1,
            6, 7,
            1, 2,
            4, 7,
            6, 5,
            5, 4,
            5, 2,
            3, 2,
            3, 4
        };

    public void Activate () {
        _cloud_status = GetComponent<CloudData>();
        _box = new GameObject("Box");
        _box.AddComponent<DragMouse>();
        _box.GetComponent<DragMouse>().enabled = !DesktopApplication.instance.VR_Enabled;
        _box.AddComponent<MeshFilter>();
        _material = new Material(Shader.Find("Unlit/Color"));
        _material.SetColor("_Color", Color.white);
        _box.AddComponent<MeshRenderer>().material = _material;
        _box.transform.SetParent(transform.parent,false);
        CreateBox();
        AddFollowComponents(this.gameObject, _box);
        AddDraggableComponents(_box);
        transform.parent.GetComponent<CloudObjectRefference>().box = _box;
    }

    private void AddDraggableComponents(GameObject go)
    {
        go.AddComponent<Rigidbody>().useGravity = false;
        go.GetComponent<Rigidbody>().isKinematic = true;
        go.AddComponent<BoxCollider>();
        go.GetComponent<BoxCollider>().isTrigger = true;
        go.GetComponent<BoxCollider>().size = Vector3.one;
        go.AddComponent<VRTK_InteractableObject>().isGrabbable = true;
        go.AddComponent<VRTK_ChildOfControllerGrabAttach>().precisionGrab = true;
        go.GetComponent<VRTK_InteractableObject>().grabAttachMechanicScript = go.GetComponent<VRTK_ChildOfControllerGrabAttach>();
        go.GetComponent<VRTK_InteractableObject>().InteractableObjectTouched += Hovered;
        go.GetComponent<VRTK_InteractableObject>().InteractableObjectUntouched += Unhovered;
        go.GetComponent<VRTK_InteractableObject>().InteractableObjectGrabbed += Grabbed;
        go.GetComponent<VRTK_InteractableObject>().InteractableObjectUngrabbed += Ungrabbed;
    }

    private void AddFollowComponents(GameObject go, GameObject object_to_follow)
    {
        go.AddComponent<VRTK_RigidbodyFollow>();
        go.GetComponent<VRTK_RigidbodyFollow>().followsScale = false;
        go.GetComponent<VRTK_RigidbodyFollow>().gameObjectToFollow = object_to_follow;
    }

    public void CreateBox()
    {
        Mesh mesh = _box.GetComponent<MeshFilter>().mesh;
        
        Vector3[] vertices = {
            new Vector3(- 0.5f, - 0.5f, - 0.5f),
            new Vector3(0.5f, - 0.5f, - 0.5f),
            new Vector3(0.5f, 0.5f, - 0.5f),
            new Vector3(- 0.5f, 0.5f, - 0.5f),
            new Vector3(- 0.5f, 0.5f,  0.5f),
            new Vector3( 0.5f,  0.5f,   0.5f),
            new Vector3( 0.5f, - 0.5f,   0.5f),
            new Vector3(- 0.5f, - 0.5f,   0.5f),
        };
    
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.SetIndices(_lines, MeshTopology.Lines, 0);

        _box.transform.localScale = _cloud_status.globalMetaData.box_scale;

        _box.tag = "PointCloud";
    }

    public void Hovered(object o, InteractableObjectEventArgs e)
    {
        if (!_is_grabbed)
        {
            _material.color = UIColors._hovered;
        }
    }

    public void Unhovered(object o, InteractableObjectEventArgs e)
    {
        if (!_is_grabbed)
        {
            _material.color = UIColors._movable;
        }
    }

    public void Grabbed(object o, InteractableObjectEventArgs e)
    {
        _is_grabbed = true;
        _material.color = UIColors._clicked;
    }

    public void Ungrabbed(object o, InteractableObjectEventArgs e)
    {
        _is_grabbed = false;
        _material.color = UIColors._hovered;
    }
}
