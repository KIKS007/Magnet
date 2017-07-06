// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.32 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.32;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:True,stva:1,stmr:255,stmw:255,stcp:4,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:3138,x:32859,y:32729,varname:node_3138,prsc:2|emission-602-OUT,alpha-260-OUT;n:type:ShaderForge.SFN_Tex2d,id:7246,x:31880,y:33088,ptovrint:False,ptlb:T_Button,ptin:_T_Button,varname:node_7246,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:2,isnm:False;n:type:ShaderForge.SFN_Multiply,id:2519,x:31880,y:33300,varname:node_2519,prsc:2|A-7246-RGB,B-879-OUT;n:type:ShaderForge.SFN_Slider,id:7649,x:32145,y:33283,ptovrint:False,ptlb:Emission Power Button,ptin:_EmissionPowerButton,varname:node_7649,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:10;n:type:ShaderForge.SFN_Multiply,id:3957,x:32224,y:33091,varname:node_3957,prsc:2|A-2519-OUT,B-7649-OUT;n:type:ShaderForge.SFN_Tex2d,id:4753,x:31258,y:32854,ptovrint:False,ptlb:T_Neon,ptin:_T_Neon,varname:node_4753,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:8805,x:31586,y:32684,varname:node_8805,prsc:2|A-4076-OUT,B-9471-OUT;n:type:ShaderForge.SFN_Time,id:5958,x:31740,y:32891,varname:node_5958,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:9817,x:31740,y:32821,ptovrint:False,ptlb:Neon Pulse Speed,ptin:_NeonPulseSpeed,varname:node_9817,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:7086,x:31930,y:32859,varname:node_7086,prsc:2|A-9817-OUT,B-5958-T;n:type:ShaderForge.SFN_Sin,id:2503,x:32110,y:32859,varname:node_2503,prsc:2|IN-7086-OUT;n:type:ShaderForge.SFN_Lerp,id:3741,x:31980,y:32612,varname:node_3741,prsc:2|A-8361-OUT,B-8805-OUT,T-2503-OUT;n:type:ShaderForge.SFN_Multiply,id:8361,x:31586,y:32479,varname:node_8361,prsc:2|A-1791-OUT,B-4753-RGB;n:type:ShaderForge.SFN_Lerp,id:602,x:32577,y:32765,varname:node_602,prsc:2|A-861-OUT,B-3957-OUT,T-7246-RGB;n:type:ShaderForge.SFN_Multiply,id:861,x:32274,y:32592,varname:node_861,prsc:2|A-1021-OUT,B-3741-OUT;n:type:ShaderForge.SFN_Slider,id:1021,x:31901,y:32513,ptovrint:False,ptlb:Emission Power Neon,ptin:_EmissionPowerNeon,varname:node_1021,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:10;n:type:ShaderForge.SFN_ComponentMask,id:260,x:32670,y:32988,varname:node_260,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-9582-OUT;n:type:ShaderForge.SFN_Add,id:9582,x:32487,y:32988,varname:node_9582,prsc:2|A-4753-RGB,B-7246-RGB;n:type:ShaderForge.SFN_Color,id:6513,x:30065,y:31459,ptovrint:False,ptlb:PURPLE CHROMA Neon1,ptin:_PURPLECHROMANeon1,varname:node_6513,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.6551723,c2:0,c3:1,c4:1;n:type:ShaderForge.SFN_Color,id:729,x:30065,y:31633,ptovrint:False,ptlb:PURPLE CHROMA Neon2,ptin:_PURPLECHROMANeon2,varname:_PURPLECHROMANeon2,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0.7867647,c3:0.932353,c4:1;n:type:ShaderForge.SFN_Color,id:9545,x:30065,y:30946,ptovrint:False,ptlb:PURPLE CHROMA Idle,ptin:_PURPLECHROMAIdle,varname:_PURPLECHROMANeon3,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0,c3:0.8896551,c4:1;n:type:ShaderForge.SFN_Color,id:7243,x:30065,y:31115,ptovrint:False,ptlb:PURPLE CHROMA Highlight,ptin:_PURPLECHROMAHighlight,varname:_PURPLECHROMAIdle_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.9943205,c2:0.5882353,c3:1,c4:1;n:type:ShaderForge.SFN_Color,id:1751,x:30065,y:31285,ptovrint:False,ptlb:PURPLE CHROMA Selection,ptin:_PURPLECHROMASelection,varname:_PURPLECHROMAHighlight_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0.8390467,c3:0.4926471,c4:1;n:type:ShaderForge.SFN_Color,id:3830,x:30067,y:32305,ptovrint:False,ptlb:BLUE CHROMA Selection,ptin:_BLUECHROMASelection,varname:_PURPLECHROMASelection_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0.7655173,c3:0,c4:1;n:type:ShaderForge.SFN_ToggleProperty,id:3506,x:29945,y:31842,ptovrint:False,ptlb:Blue Chroma,ptin:_BlueChroma,varname:node_3506,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False;n:type:ShaderForge.SFN_Lerp,id:6294,x:30499,y:31534,varname:node_6294,prsc:2|A-9545-RGB,B-1236-RGB,T-3506-OUT;n:type:ShaderForge.SFN_Lerp,id:3930,x:30499,y:31674,varname:node_3930,prsc:2|A-7243-RGB,B-9971-RGB,T-3506-OUT;n:type:ShaderForge.SFN_Lerp,id:4719,x:30499,y:31812,varname:node_4719,prsc:2|A-1751-RGB,B-3830-RGB,T-3506-OUT;n:type:ShaderForge.SFN_Lerp,id:7953,x:30499,y:31945,varname:node_7953,prsc:2|A-6513-RGB,B-2898-RGB,T-3506-OUT;n:type:ShaderForge.SFN_Lerp,id:8781,x:30499,y:32082,varname:node_8781,prsc:2|A-729-RGB,B-2605-RGB,T-3506-OUT;n:type:ShaderForge.SFN_Lerp,id:8615,x:30506,y:32571,varname:node_8615,prsc:2|A-6294-OUT,B-2889-RGB,T-669-OUT;n:type:ShaderForge.SFN_Lerp,id:6791,x:30506,y:32710,varname:node_6791,prsc:2|A-3930-OUT,B-2708-RGB,T-669-OUT;n:type:ShaderForge.SFN_Lerp,id:5964,x:30506,y:32848,varname:node_5964,prsc:2|A-4719-OUT,B-4964-RGB,T-669-OUT;n:type:ShaderForge.SFN_Lerp,id:9525,x:30506,y:32982,varname:node_9525,prsc:2|A-7953-OUT,B-2399-RGB,T-669-OUT;n:type:ShaderForge.SFN_Lerp,id:4076,x:30506,y:33118,varname:node_4076,prsc:2|A-8781-OUT,B-2193-RGB,T-669-OUT;n:type:ShaderForge.SFN_Color,id:2898,x:30067,y:32479,ptovrint:False,ptlb:BLUE CHROMA Neon1,ptin:_BLUECHROMANeon1,varname:_lol_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0.04827595,c3:1,c4:1;n:type:ShaderForge.SFN_Color,id:2605,x:30067,y:32664,ptovrint:False,ptlb:BLUE CHROMA Neon2,ptin:_BLUECHROMANeon2,varname:_BLUECHROMANeon2,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.7279412,c2:0.7407439,c3:1,c4:1;n:type:ShaderForge.SFN_ToggleProperty,id:669,x:29953,y:32880,ptovrint:False,ptlb:Green Chroma,ptin:_GreenChroma,varname:node_669,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False;n:type:ShaderForge.SFN_Color,id:2889,x:30061,y:33015,ptovrint:False,ptlb:GREEN CHROMA Idle,ptin:_GREENCHROMAIdle,varname:node_2889,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0.8382353,c3:0.4566936,c4:1;n:type:ShaderForge.SFN_Color,id:2708,x:30061,y:33189,ptovrint:False,ptlb:GREEN CHROMA Highlight,ptin:_GREENCHROMAHighlight,varname:node_2708,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.3014706,c2:1,c3:0.5086207,c4:1;n:type:ShaderForge.SFN_Color,id:4964,x:30061,y:33369,ptovrint:False,ptlb:GREEN CHROMA Selection,ptin:_GREENCHROMASelection,varname:node_4964,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.8154159,c2:1,c3:0.5220588,c4:1;n:type:ShaderForge.SFN_Color,id:2399,x:30061,y:33549,ptovrint:False,ptlb:GREEN CHROMA Neon1,ptin:_GREENCHROMANeon1,varname:node_2399,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.04827586,c2:0.4117647,c3:0,c4:1;n:type:ShaderForge.SFN_Color,id:2193,x:30061,y:33729,ptovrint:False,ptlb:GREEN CHROMA Neon2,ptin:_GREENCHROMANeon2,varname:node_2193,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.7858012,c2:1,c3:0.7573529,c4:1;n:type:ShaderForge.SFN_Lerp,id:7968,x:30507,y:33658,varname:node_7968,prsc:2|A-8615-OUT,B-9296-RGB,T-459-OUT;n:type:ShaderForge.SFN_Lerp,id:1997,x:30507,y:33797,varname:node_1997,prsc:2|A-6791-OUT,B-4898-RGB,T-459-OUT;n:type:ShaderForge.SFN_Lerp,id:9154,x:30507,y:33935,varname:node_9154,prsc:2|A-5964-OUT,B-8962-RGB,T-459-OUT;n:type:ShaderForge.SFN_Lerp,id:1791,x:30507,y:34069,varname:node_1791,prsc:2|A-9525-OUT,B-645-RGB,T-459-OUT;n:type:ShaderForge.SFN_Lerp,id:9471,x:30507,y:34205,varname:node_9471,prsc:2|A-4076-OUT,B-404-RGB,T-459-OUT;n:type:ShaderForge.SFN_ToggleProperty,id:459,x:29937,y:33948,ptovrint:False,ptlb:Orange Chroma,ptin:_OrangeChroma,varname:node_459,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False;n:type:ShaderForge.SFN_Color,id:9296,x:30062,y:34083,ptovrint:False,ptlb:ORANGE CHROMA Idle,ptin:_ORANGECHROMAIdle,varname:_node_2193_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0.4758621,c3:0,c4:1;n:type:ShaderForge.SFN_Color,id:4898,x:30062,y:34261,ptovrint:False,ptlb:ORANGE CHROMA Highlight,ptin:_ORANGECHROMAHighlight,varname:_node_2193_copy_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0.7811359,c3:0.3897059,c4:1;n:type:ShaderForge.SFN_Color,id:8962,x:30062,y:34442,ptovrint:False,ptlb:ORANGE CHROMA Selection,ptin:_ORANGECHROMASelection,varname:_node_2193_copy_copy_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0.9497972,c3:0.6691177,c4:1;n:type:ShaderForge.SFN_Color,id:645,x:30062,y:34625,ptovrint:False,ptlb:ORANGE CHROMA Neon1,ptin:_ORANGECHROMANeon1,varname:_node_2193_copy_copy_copy_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0.2689655,c3:0,c4:1;n:type:ShaderForge.SFN_Color,id:404,x:30062,y:34807,ptovrint:False,ptlb:ORANGE CHROMA Neon2,ptin:_ORANGECHROMANeon2,varname:_node_2193_copy_copy_copy_copy_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0.7754057,c3:0.6838235,c4:1;n:type:ShaderForge.SFN_ToggleProperty,id:4178,x:31225,y:33717,ptovrint:False,ptlb:Selection,ptin:_Selection,varname:node_4178,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False;n:type:ShaderForge.SFN_ToggleProperty,id:9676,x:31225,y:33417,ptovrint:False,ptlb:Highlighting,ptin:_Highlighting,varname:node_9676,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False;n:type:ShaderForge.SFN_Lerp,id:9567,x:31225,y:33208,varname:node_9567,prsc:2|A-7968-OUT,B-1997-OUT,T-9676-OUT;n:type:ShaderForge.SFN_Lerp,id:879,x:31225,y:33505,varname:node_879,prsc:2|A-9567-OUT,B-9154-OUT,T-4178-OUT;n:type:ShaderForge.SFN_Color,id:1236,x:30067,y:31959,ptovrint:False,ptlb:BLUE CHROMA Idle,ptin:_BLUECHROMAIdle,varname:_BLUECHROMAIdle_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0.5448277,c3:1,c4:1;n:type:ShaderForge.SFN_Color,id:9971,x:30067,y:32133,ptovrint:False,ptlb:BLUE CHROMA Highlight,ptin:_BLUECHROMAHighlight,varname:_BLUECHROMAHighlight_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0.751724,c3:1,c4:1;n:type:ShaderForge.SFN_Color,id:3102,x:30195,y:32263,ptovrint:False,ptlb:BLUE CHROMA Highlight_copy,ptin:_BLUECHROMAHighlight_copy,varname:_BLUECHROMAHighlight_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0.751724,c3:1,c4:1;n:type:ShaderForge.SFN_Color,id:3100,x:30259,y:32327,ptovrint:False,ptlb:BLUE CHROMA Highlight_copy_copy,ptin:_BLUECHROMAHighlight_copy_copy,varname:_BLUECHROMAHighlight_copy_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0.751724,c3:1,c4:1;n:type:ShaderForge.SFN_Color,id:5943,x:30323,y:32391,ptovrint:False,ptlb:BLUE CHROMA Highlight_copy_copy_copy,ptin:_BLUECHROMAHighlight_copy_copy_copy,varname:_BLUECHROMAHighlight_copy_copy_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0.751724,c3:1,c4:1;n:type:ShaderForge.SFN_Color,id:5975,x:30387,y:32455,ptovrint:False,ptlb:BLUE CHROMA Highlight_copy_copy_copy_copy,ptin:_BLUECHROMAHighlight_copy_copy_copy_copy,varname:_BLUECHROMAHighlight_copy_copy_copy_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0.751724,c3:1,c4:1;n:type:ShaderForge.SFN_Color,id:6621,x:30451,y:32519,ptovrint:False,ptlb:BLUE CHROMA Highlight_copy_copy_copy_copy_copy,ptin:_BLUECHROMAHighlight_copy_copy_copy_copy_copy,varname:_BLUECHROMAHighlight_copy_copy_copy_copy_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0.751724,c3:1,c4:1;proporder:7246-7649-4753-1021-9817-3506-669-459-9676-4178-9545-7243-1751-6513-729-1236-9971-3830-2898-2605-2889-2708-4964-2399-2193-9296-4898-8962-645-404;pass:END;sub:END;*/

Shader "Shader Forge/ButtonTest" {
    Properties {
        _T_Button ("T_Button", 2D) = "black" {}
        _EmissionPowerButton ("Emission Power Button", Range(0, 10)) = 1
        _T_Neon ("T_Neon", 2D) = "white" {}
        _EmissionPowerNeon ("Emission Power Neon", Range(0, 10)) = 1
        _NeonPulseSpeed ("Neon Pulse Speed", Float ) = 1
        [MaterialToggle] _BlueChroma ("Blue Chroma", Float ) = 0
        [MaterialToggle] _GreenChroma ("Green Chroma", Float ) = 0
        [MaterialToggle] _OrangeChroma ("Orange Chroma", Float ) = 0
        [MaterialToggle] _Highlighting ("Highlighting", Float ) = 0
        [MaterialToggle] _Selection ("Selection", Float ) = 0
        _PURPLECHROMAIdle ("PURPLE CHROMA Idle", Color) = (1,0,0.8896551,1)
        _PURPLECHROMAHighlight ("PURPLE CHROMA Highlight", Color) = (0.9943205,0.5882353,1,1)
        _PURPLECHROMASelection ("PURPLE CHROMA Selection", Color) = (1,0.8390467,0.4926471,1)
        _PURPLECHROMANeon1 ("PURPLE CHROMA Neon1", Color) = (0.6551723,0,1,1)
        _PURPLECHROMANeon2 ("PURPLE CHROMA Neon2", Color) = (1,0.7867647,0.932353,1)
        _BLUECHROMAIdle ("BLUE CHROMA Idle", Color) = (0,0.5448277,1,1)
        _BLUECHROMAHighlight ("BLUE CHROMA Highlight", Color) = (0,0.751724,1,1)
        _BLUECHROMASelection ("BLUE CHROMA Selection", Color) = (1,0.7655173,0,1)
        _BLUECHROMANeon1 ("BLUE CHROMA Neon1", Color) = (0,0.04827595,1,1)
        _BLUECHROMANeon2 ("BLUE CHROMA Neon2", Color) = (0.7279412,0.7407439,1,1)
        _GREENCHROMAIdle ("GREEN CHROMA Idle", Color) = (0,0.8382353,0.4566936,1)
        _GREENCHROMAHighlight ("GREEN CHROMA Highlight", Color) = (0.3014706,1,0.5086207,1)
        _GREENCHROMASelection ("GREEN CHROMA Selection", Color) = (0.8154159,1,0.5220588,1)
        _GREENCHROMANeon1 ("GREEN CHROMA Neon1", Color) = (0.04827586,0.4117647,0,1)
        _GREENCHROMANeon2 ("GREEN CHROMA Neon2", Color) = (0.7858012,1,0.7573529,1)
        _ORANGECHROMAIdle ("ORANGE CHROMA Idle", Color) = (1,0.4758621,0,1)
        _ORANGECHROMAHighlight ("ORANGE CHROMA Highlight", Color) = (1,0.7811359,0.3897059,1)
        _ORANGECHROMASelection ("ORANGE CHROMA Selection", Color) = (1,0.9497972,0.6691177,1)
        _ORANGECHROMANeon1 ("ORANGE CHROMA Neon1", Color) = (1,0.2689655,0,1)
        _ORANGECHROMANeon2 ("ORANGE CHROMA Neon2", Color) = (1,0.7754057,0.6838235,1)
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            Stencil {
                Ref 1
                Comp Equal
            }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _T_Button; uniform float4 _T_Button_ST;
            uniform float _EmissionPowerButton;
            uniform sampler2D _T_Neon; uniform float4 _T_Neon_ST;
            uniform float _NeonPulseSpeed;
            uniform float _EmissionPowerNeon;
            uniform float4 _PURPLECHROMANeon1;
            uniform float4 _PURPLECHROMANeon2;
            uniform float4 _PURPLECHROMAIdle;
            uniform float4 _PURPLECHROMAHighlight;
            uniform float4 _PURPLECHROMASelection;
            uniform float4 _BLUECHROMASelection;
            uniform fixed _BlueChroma;
            uniform float4 _BLUECHROMANeon1;
            uniform float4 _BLUECHROMANeon2;
            uniform fixed _GreenChroma;
            uniform float4 _GREENCHROMAIdle;
            uniform float4 _GREENCHROMAHighlight;
            uniform float4 _GREENCHROMASelection;
            uniform float4 _GREENCHROMANeon1;
            uniform float4 _GREENCHROMANeon2;
            uniform fixed _OrangeChroma;
            uniform float4 _ORANGECHROMAIdle;
            uniform float4 _ORANGECHROMAHighlight;
            uniform float4 _ORANGECHROMASelection;
            uniform float4 _ORANGECHROMANeon1;
            uniform float4 _ORANGECHROMANeon2;
            uniform fixed _Selection;
            uniform fixed _Highlighting;
            uniform float4 _BLUECHROMAIdle;
            uniform float4 _BLUECHROMAHighlight;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos(v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float4 _T_Neon_var = tex2D(_T_Neon,TRANSFORM_TEX(i.uv0, _T_Neon));
                float3 node_4076 = lerp(lerp(_PURPLECHROMANeon2.rgb,_BLUECHROMANeon2.rgb,_BlueChroma),_GREENCHROMANeon2.rgb,_GreenChroma);
                float4 node_5958 = _Time + _TimeEditor;
                float4 _T_Button_var = tex2D(_T_Button,TRANSFORM_TEX(i.uv0, _T_Button));
                float3 emissive = lerp((_EmissionPowerNeon*lerp((lerp(lerp(lerp(_PURPLECHROMANeon1.rgb,_BLUECHROMANeon1.rgb,_BlueChroma),_GREENCHROMANeon1.rgb,_GreenChroma),_ORANGECHROMANeon1.rgb,_OrangeChroma)*_T_Neon_var.rgb),(node_4076*lerp(node_4076,_ORANGECHROMANeon2.rgb,_OrangeChroma)),sin((_NeonPulseSpeed*node_5958.g)))),((_T_Button_var.rgb*lerp(lerp(lerp(lerp(lerp(_PURPLECHROMAIdle.rgb,_BLUECHROMAIdle.rgb,_BlueChroma),_GREENCHROMAIdle.rgb,_GreenChroma),_ORANGECHROMAIdle.rgb,_OrangeChroma),lerp(lerp(lerp(_PURPLECHROMAHighlight.rgb,_BLUECHROMAHighlight.rgb,_BlueChroma),_GREENCHROMAHighlight.rgb,_GreenChroma),_ORANGECHROMAHighlight.rgb,_OrangeChroma),_Highlighting),lerp(lerp(lerp(_PURPLECHROMASelection.rgb,_BLUECHROMASelection.rgb,_BlueChroma),_GREENCHROMASelection.rgb,_GreenChroma),_ORANGECHROMASelection.rgb,_OrangeChroma),_Selection))*_EmissionPowerButton),_T_Button_var.rgb);
                float3 finalColor = emissive;
                return fixed4(finalColor,(_T_Neon_var.rgb+_T_Button_var.rgb).r);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
