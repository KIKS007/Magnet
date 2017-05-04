// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.32 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.32;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:1,trmd:1,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:4013,x:42134,y:30590,varname:node_4013,prsc:2|diff-3029-OUT,emission-8293-OUT,alpha-9783-OUT;n:type:ShaderForge.SFN_Tex2d,id:1555,x:34441,y:30468,ptovrint:False,ptlb:T_Lava01,ptin:_T_Lava01,varname:node_1555,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:28c17e75ad3dcb7468d177c1f6b86ac1,ntxv:0,isnm:False|UVIN-3515-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:2932,x:34441,y:30651,ptovrint:False,ptlb:T_Lava02,ptin:_T_Lava02,varname:node_2932,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:64bcf5ba77b515841af46bf7980e6064,ntxv:0,isnm:False|UVIN-5482-UVOUT;n:type:ShaderForge.SFN_Lerp,id:6428,x:35121,y:30855,varname:node_6428,prsc:2|A-5564-OUT,B-8911-OUT,T-3892-RGB;n:type:ShaderForge.SFN_Tex2d,id:3892,x:34441,y:30842,ptovrint:False,ptlb:T_Lavalpha02,ptin:_T_Lavalpha02,varname:node_3892,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:03210bed239f8154a87180c5a248e5e3,ntxv:0,isnm:False|UVIN-5482-UVOUT;n:type:ShaderForge.SFN_Panner,id:5482,x:34206,y:30651,varname:node_5482,prsc:2,spu:0.5,spv:0.3|UVIN-6709-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:6709,x:33960,y:30651,varname:node_6709,prsc:2,uv:0;n:type:ShaderForge.SFN_Tex2d,id:1771,x:34441,y:31039,ptovrint:False,ptlb:T_Lava03,ptin:_T_Lava03,varname:node_1771,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:eb2233c54dfc6f845bfe43fe04d7d340,ntxv:0,isnm:False|UVIN-4922-UVOUT;n:type:ShaderForge.SFN_Panner,id:3515,x:34206,y:30468,varname:node_3515,prsc:2,spu:0,spv:0.1|UVIN-4224-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:4224,x:33960,y:30468,varname:node_4224,prsc:2,uv:0;n:type:ShaderForge.SFN_Tex2d,id:9258,x:34446,y:31231,ptovrint:False,ptlb:T_Lavalpha03,ptin:_T_Lavalpha03,varname:node_9258,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:0c802e14264713d479db105839971052,ntxv:0,isnm:False|UVIN-4922-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:216,x:34447,y:31426,ptovrint:False,ptlb:T_Lava04,ptin:_T_Lava04,varname:node_216,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:2e0856832b820c64585aff91694fa027,ntxv:0,isnm:False|UVIN-3039-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:9117,x:34447,y:31619,ptovrint:False,ptlb:T_Lavalpha04,ptin:_T_Lavalpha04,varname:node_9117,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:a9be5c57f93d8054f80577e3a08cce5b,ntxv:0,isnm:False|UVIN-3039-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:6596,x:34447,y:31816,ptovrint:False,ptlb:T_Lava05,ptin:_T_Lava05,varname:node_6596,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:c4000dc48317f5344b78352d54528712,ntxv:0,isnm:False|UVIN-3076-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:4785,x:34447,y:32007,ptovrint:False,ptlb:T_Lavalpha05,ptin:_T_Lavalpha05,varname:node_4785,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:65653538d87aa2d4ebba08df5a3ae447,ntxv:0,isnm:False|UVIN-3076-UVOUT;n:type:ShaderForge.SFN_Lerp,id:1877,x:35116,y:31187,varname:node_1877,prsc:2|A-6428-OUT,B-3599-OUT,T-9258-RGB;n:type:ShaderForge.SFN_Lerp,id:8188,x:35116,y:31536,varname:node_8188,prsc:2|A-1877-OUT,B-5374-OUT,T-9117-RGB;n:type:ShaderForge.SFN_Lerp,id:119,x:35117,y:31872,varname:node_119,prsc:2|A-8188-OUT,B-7978-OUT,T-4785-RGB;n:type:ShaderForge.SFN_Panner,id:4922,x:34205,y:31039,varname:node_4922,prsc:2,spu:0,spv:0.2|UVIN-2520-UVOUT;n:type:ShaderForge.SFN_Panner,id:3039,x:34208,y:31426,varname:node_3039,prsc:2,spu:1,spv:0.5|UVIN-1837-UVOUT;n:type:ShaderForge.SFN_Panner,id:3076,x:34212,y:31816,varname:node_3076,prsc:2,spu:0,spv:0.4|UVIN-3067-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:2520,x:33959,y:31039,varname:node_2520,prsc:2,uv:0;n:type:ShaderForge.SFN_TexCoord,id:1837,x:33958,y:31426,varname:node_1837,prsc:2,uv:0;n:type:ShaderForge.SFN_TexCoord,id:3067,x:33959,y:31816,varname:node_3067,prsc:2,uv:0;n:type:ShaderForge.SFN_Color,id:456,x:34767,y:30201,ptovrint:False,ptlb:LavaBLUE1,ptin:_LavaBLUE1,varname:node_456,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.25,c2:0.8758624,c3:1,c4:1;n:type:ShaderForge.SFN_Add,id:5564,x:34767,y:30380,varname:node_5564,prsc:2|A-456-RGB,B-1555-RGB;n:type:ShaderForge.SFN_Add,id:8911,x:34768,y:30731,varname:node_8911,prsc:2|A-5060-RGB,B-2932-RGB;n:type:ShaderForge.SFN_Color,id:5060,x:34768,y:30557,ptovrint:False,ptlb:LavaBLUE2,ptin:_LavaBLUE2,varname:node_5060,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.3235294,c2:0.8880324,c3:1,c4:1;n:type:ShaderForge.SFN_Add,id:3599,x:34774,y:31112,varname:node_3599,prsc:2|A-1475-RGB,B-1771-RGB;n:type:ShaderForge.SFN_Color,id:1475,x:34774,y:30934,ptovrint:False,ptlb:LavaBLUE3,ptin:_LavaBLUE3,varname:node_1475,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0.4206896,c3:1,c4:1;n:type:ShaderForge.SFN_Add,id:5374,x:34778,y:31520,varname:node_5374,prsc:2|A-1010-RGB,B-216-RGB;n:type:ShaderForge.SFN_Color,id:1010,x:34778,y:31334,ptovrint:False,ptlb:LavaBLUE4,ptin:_LavaBLUE4,varname:node_1010,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0.5862069,c3:1,c4:1;n:type:ShaderForge.SFN_Add,id:7978,x:34778,y:31901,varname:node_7978,prsc:2|A-7542-RGB,B-6596-RGB;n:type:ShaderForge.SFN_Color,id:7542,x:34778,y:31721,ptovrint:False,ptlb:LavaBLUE5,ptin:_LavaBLUE5,varname:node_7542,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0.3793104,c3:1,c4:1;n:type:ShaderForge.SFN_Color,id:4062,x:38533,y:30773,ptovrint:False,ptlb:EmmisionBLUE,ptin:_EmmisionBLUE,varname:node_4062,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0.9172413,c3:1,c4:1;n:type:ShaderForge.SFN_Lerp,id:5649,x:37761,y:30133,varname:node_5649,prsc:2|A-1635-OUT,B-9218-OUT,T-3892-RGB;n:type:ShaderForge.SFN_Lerp,id:1079,x:37762,y:30458,varname:node_1079,prsc:2|A-5649-OUT,B-2666-OUT,T-9258-RGB;n:type:ShaderForge.SFN_Lerp,id:6719,x:37763,y:30776,varname:node_6719,prsc:2|A-1079-OUT,B-5104-OUT,T-9117-RGB;n:type:ShaderForge.SFN_Lerp,id:3274,x:37764,y:31096,varname:node_3274,prsc:2|A-6719-OUT,B-470-OUT,T-4785-RGB;n:type:ShaderForge.SFN_Add,id:1635,x:37374,y:29923,varname:node_1635,prsc:2|A-1632-RGB,B-1555-RGB;n:type:ShaderForge.SFN_Add,id:9218,x:37374,y:30050,varname:node_9218,prsc:2|A-1632-RGB,B-2932-RGB;n:type:ShaderForge.SFN_Add,id:2666,x:37374,y:30284,varname:node_2666,prsc:2|A-1632-RGB,B-1771-RGB;n:type:ShaderForge.SFN_Add,id:5104,x:37374,y:30406,varname:node_5104,prsc:2|A-1632-RGB,B-216-RGB;n:type:ShaderForge.SFN_Add,id:470,x:37374,y:30641,varname:node_470,prsc:2|A-1632-RGB,B-6596-RGB;n:type:ShaderForge.SFN_Color,id:1632,x:37036,y:30297,ptovrint:False,ptlb:LavaNEUTRAL,ptin:_LavaNEUTRAL,varname:_Color_Lava06,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Lerp,id:2573,x:37431,y:31410,varname:node_2573,prsc:2|A-3274-OUT,B-119-OUT,T-8748-OUT;n:type:ShaderForge.SFN_Slider,id:8748,x:37045,y:31609,ptovrint:False,ptlb:LerpBLUE,ptin:_LerpBLUE,varname:node_8748,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Color,id:7486,x:38533,y:30587,ptovrint:False,ptlb:EmissionNEUTRAL,ptin:_EmissionNEUTRAL,varname:node_7486,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Lerp,id:1860,x:38852,y:30682,varname:node_1860,prsc:2|A-7486-RGB,B-4062-RGB,T-8748-OUT;n:type:ShaderForge.SFN_Slider,id:9783,x:41063,y:30609,ptovrint:False,ptlb:Opacity,ptin:_Opacity,varname:node_9783,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Lerp,id:9362,x:35336,y:33405,varname:node_9362,prsc:2|A-2236-OUT,B-6180-OUT,T-3892-RGB;n:type:ShaderForge.SFN_Lerp,id:4179,x:35331,y:33737,varname:node_4179,prsc:2|A-9362-OUT,B-7791-OUT,T-9258-RGB;n:type:ShaderForge.SFN_Lerp,id:8031,x:35331,y:34086,varname:node_8031,prsc:2|A-4179-OUT,B-1988-OUT,T-9117-RGB;n:type:ShaderForge.SFN_Lerp,id:9933,x:35332,y:34422,varname:node_9933,prsc:2|A-8031-OUT,B-9694-OUT,T-4785-RGB;n:type:ShaderForge.SFN_Color,id:6612,x:34982,y:32751,ptovrint:False,ptlb:LavaGREEN1,ptin:_LavaGREEN1,varname:_LavaBLUE2,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.3068966,c2:1,c3:0.25,c4:1;n:type:ShaderForge.SFN_Add,id:2236,x:34982,y:32930,varname:node_2236,prsc:2|A-6612-RGB,B-1555-RGB;n:type:ShaderForge.SFN_Add,id:6180,x:34983,y:33281,varname:node_6180,prsc:2|A-4729-RGB,B-2932-RGB;n:type:ShaderForge.SFN_Color,id:4729,x:34983,y:33107,ptovrint:False,ptlb:LavaGREEN2,ptin:_LavaGREEN2,varname:_LavaBLUE3,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0.6985294,c3:0.09153153,c4:1;n:type:ShaderForge.SFN_Add,id:7791,x:34989,y:33662,varname:node_7791,prsc:2|A-8637-RGB,B-1771-RGB;n:type:ShaderForge.SFN_Color,id:8637,x:34989,y:33484,ptovrint:False,ptlb:LavaGREEN3,ptin:_LavaGREEN3,varname:_LavaBLUE4,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.03448272,c2:1,c3:0,c4:1;n:type:ShaderForge.SFN_Add,id:1988,x:34993,y:34070,varname:node_1988,prsc:2|A-341-RGB,B-216-RGB;n:type:ShaderForge.SFN_Color,id:341,x:34993,y:33884,ptovrint:False,ptlb:LavaGREEN4,ptin:_LavaGREEN4,varname:_LavaBLUE5,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.04629824,c2:0.6102941,c3:0,c4:1;n:type:ShaderForge.SFN_Add,id:9694,x:34993,y:34451,varname:node_9694,prsc:2|A-2426-RGB,B-6596-RGB;n:type:ShaderForge.SFN_Color,id:2426,x:34993,y:34271,ptovrint:False,ptlb:LavaGREEN5,ptin:_LavaGREEN5,varname:_LavaBLUE6,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0.8676471,c3:0.2213995,c4:1;n:type:ShaderForge.SFN_Lerp,id:8300,x:37174,y:34698,varname:node_8300,prsc:2|A-1223-OUT,B-976-OUT,T-3892-RGB;n:type:ShaderForge.SFN_Lerp,id:2110,x:37169,y:35030,varname:node_2110,prsc:2|A-8300-OUT,B-1214-OUT,T-9258-RGB;n:type:ShaderForge.SFN_Lerp,id:3099,x:37169,y:35379,varname:node_3099,prsc:2|A-2110-OUT,B-6289-OUT,T-9117-RGB;n:type:ShaderForge.SFN_Lerp,id:1535,x:37170,y:35715,varname:node_1535,prsc:2|A-3099-OUT,B-3937-OUT,T-4785-RGB;n:type:ShaderForge.SFN_Add,id:1223,x:36820,y:34223,varname:node_1223,prsc:2|A-2206-RGB,B-1555-RGB;n:type:ShaderForge.SFN_Add,id:976,x:36821,y:34574,varname:node_976,prsc:2|A-6867-RGB,B-2932-RGB;n:type:ShaderForge.SFN_Add,id:1214,x:36827,y:34955,varname:node_1214,prsc:2|A-5627-RGB,B-1771-RGB;n:type:ShaderForge.SFN_Add,id:6289,x:36831,y:35363,varname:node_6289,prsc:2|A-528-RGB,B-216-RGB;n:type:ShaderForge.SFN_Add,id:3937,x:36831,y:35744,varname:node_3937,prsc:2|A-6604-RGB,B-6596-RGB;n:type:ShaderForge.SFN_Color,id:6604,x:36831,y:35564,ptovrint:False,ptlb:LavaPINK5,ptin:_LavaPINK5,varname:_LavaBLUE7,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0,c3:0.3103447,c4:1;n:type:ShaderForge.SFN_Lerp,id:5882,x:39268,y:34195,varname:node_5882,prsc:2|A-2436-OUT,B-7280-OUT,T-3892-RGB;n:type:ShaderForge.SFN_Lerp,id:4048,x:39263,y:34527,varname:node_4048,prsc:2|A-5882-OUT,B-2419-OUT,T-9258-RGB;n:type:ShaderForge.SFN_Lerp,id:8660,x:39263,y:34876,varname:node_8660,prsc:2|A-4048-OUT,B-4319-OUT,T-9117-RGB;n:type:ShaderForge.SFN_Lerp,id:7222,x:39264,y:35212,varname:node_7222,prsc:2|A-8660-OUT,B-3995-OUT,T-4785-RGB;n:type:ShaderForge.SFN_Add,id:2436,x:38914,y:33720,varname:node_2436,prsc:2|A-9299-RGB,B-1555-RGB;n:type:ShaderForge.SFN_Add,id:7280,x:38915,y:34071,varname:node_7280,prsc:2|A-5521-RGB,B-2932-RGB;n:type:ShaderForge.SFN_Add,id:2419,x:38921,y:34452,varname:node_2419,prsc:2|A-1380-RGB,B-1771-RGB;n:type:ShaderForge.SFN_Add,id:4319,x:38925,y:34860,varname:node_4319,prsc:2|A-5832-RGB,B-216-RGB;n:type:ShaderForge.SFN_Add,id:3995,x:38925,y:35241,varname:node_3995,prsc:2|A-9808-RGB,B-6596-RGB;n:type:ShaderForge.SFN_Color,id:9808,x:38925,y:35061,ptovrint:False,ptlb:LavaYELLOW5,ptin:_LavaYELLOW5,varname:_LavaBLUE8,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0.7655172,c3:0,c4:1;n:type:ShaderForge.SFN_Lerp,id:7517,x:41144,y:32605,varname:node_7517,prsc:2|A-7997-OUT,B-8857-OUT,T-3892-RGB;n:type:ShaderForge.SFN_Lerp,id:5158,x:41139,y:32937,varname:node_5158,prsc:2|A-7517-OUT,B-8065-OUT,T-9258-RGB;n:type:ShaderForge.SFN_Lerp,id:9250,x:41139,y:33286,varname:node_9250,prsc:2|A-5158-OUT,B-6071-OUT,T-9117-RGB;n:type:ShaderForge.SFN_Lerp,id:3434,x:41140,y:33622,varname:node_3434,prsc:2|A-9250-OUT,B-7653-OUT,T-4785-RGB;n:type:ShaderForge.SFN_Add,id:7997,x:40790,y:32130,varname:node_7997,prsc:2|A-1421-RGB,B-1555-RGB;n:type:ShaderForge.SFN_Add,id:8857,x:40791,y:32481,varname:node_8857,prsc:2|A-5700-RGB,B-2932-RGB;n:type:ShaderForge.SFN_Add,id:8065,x:40797,y:32862,varname:node_8065,prsc:2|A-8088-RGB,B-1771-RGB;n:type:ShaderForge.SFN_Add,id:6071,x:40801,y:33270,varname:node_6071,prsc:2|A-3749-RGB,B-216-RGB;n:type:ShaderForge.SFN_Add,id:7653,x:40801,y:33651,varname:node_7653,prsc:2|A-1888-RGB,B-6596-RGB;n:type:ShaderForge.SFN_Color,id:1888,x:40801,y:33471,ptovrint:False,ptlb:LavaRED5,ptin:_LavaRED5,varname:_LavaBLUE9,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5955882,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Lerp,id:3688,x:36442,y:32825,varname:node_3688,prsc:2|A-3274-OUT,B-9933-OUT,T-720-OUT;n:type:ShaderForge.SFN_Slider,id:720,x:36205,y:33119,ptovrint:False,ptlb:LerpGREEN,ptin:_LerpGREEN,varname:node_720,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Lerp,id:2040,x:37647,y:33554,varname:node_2040,prsc:2|A-3274-OUT,B-1535-OUT,T-2851-OUT;n:type:ShaderForge.SFN_Lerp,id:6922,x:39647,y:33291,varname:node_6922,prsc:2|A-3274-OUT,B-7222-OUT,T-3325-OUT;n:type:ShaderForge.SFN_Lerp,id:3029,x:41608,y:31905,varname:node_3029,prsc:2|A-2848-OUT,B-3434-OUT,T-5371-OUT;n:type:ShaderForge.SFN_Slider,id:2851,x:37405,y:33877,ptovrint:False,ptlb:LerpPINK,ptin:_LerpPINK,varname:node_2851,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:3325,x:39480,y:33870,ptovrint:False,ptlb:LerpYELLOW,ptin:_LerpYELLOW,varname:node_3325,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:5371,x:41515,y:32336,ptovrint:False,ptlb:LerpRED,ptin:_LerpRED,varname:node_5371,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Lerp,id:2353,x:37912,y:31731,varname:node_2353,prsc:2|A-2573-OUT,B-2040-OUT,T-2851-OUT;n:type:ShaderForge.SFN_Lerp,id:8142,x:38517,y:32235,varname:node_8142,prsc:2|A-2353-OUT,B-3688-OUT,T-720-OUT;n:type:ShaderForge.SFN_Color,id:2206,x:36820,y:34056,ptovrint:False,ptlb:LavaPINK1,ptin:_LavaPINK1,varname:node_2206,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0,c3:0.7241378,c4:1;n:type:ShaderForge.SFN_Color,id:6867,x:36821,y:34404,ptovrint:False,ptlb:LavaPINK2,ptin:_LavaPINK2,varname:node_6867,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0,c3:0.3103447,c4:1;n:type:ShaderForge.SFN_Color,id:5627,x:36827,y:34786,ptovrint:False,ptlb:LavaPINK3,ptin:_LavaPINK3,varname:node_5627,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0.1691176,c3:0.4269776,c4:1;n:type:ShaderForge.SFN_Color,id:528,x:36831,y:35180,ptovrint:False,ptlb:LavaPINK4,ptin:_LavaPINK4,varname:node_528,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0,c3:0.3517241,c4:1;n:type:ShaderForge.SFN_Lerp,id:2848,x:39760,y:32352,varname:node_2848,prsc:2|A-8142-OUT,B-6922-OUT,T-3325-OUT;n:type:ShaderForge.SFN_Color,id:9299,x:38914,y:33550,ptovrint:False,ptlb:LavaYELLOW1,ptin:_LavaYELLOW1,varname:node_9299,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0.7241379,c3:0,c4:1;n:type:ShaderForge.SFN_Color,id:5521,x:38915,y:33906,ptovrint:False,ptlb:LavaYELLOW2,ptin:_LavaYELLOW2,varname:node_5521,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0.8068966,c3:0,c4:1;n:type:ShaderForge.SFN_Color,id:1380,x:38923,y:34291,ptovrint:False,ptlb:LavaYELLOW3,ptin:_LavaYELLOW3,varname:node_1380,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0.6,c3:0,c4:1;n:type:ShaderForge.SFN_Color,id:5832,x:38925,y:34688,ptovrint:False,ptlb:LavaYELLOW4,ptin:_LavaYELLOW4,varname:node_5832,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0.8482759,c3:0,c4:1;n:type:ShaderForge.SFN_Color,id:1421,x:40792,y:31946,ptovrint:False,ptlb:LavaRED1,ptin:_LavaRED1,varname:node_1421,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0.2132353,c3:0.2132353,c4:1;n:type:ShaderForge.SFN_Color,id:5700,x:40791,y:32310,ptovrint:False,ptlb:LavaRED2,ptin:_LavaRED2,varname:node_5700,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Color,id:8088,x:40797,y:32683,ptovrint:False,ptlb:LavaRED3,ptin:_LavaRED3,varname:node_8088,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.6764706,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Color,id:3749,x:40801,y:33068,ptovrint:False,ptlb:LavaRED4,ptin:_LavaRED4,varname:node_3749,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.8455882,c2:0.08082826,c3:0.08082826,c4:1;n:type:ShaderForge.SFN_Lerp,id:870,x:38939,y:31086,varname:node_870,prsc:2|A-1860-OUT,B-1662-RGB,T-2851-OUT;n:type:ShaderForge.SFN_Color,id:1662,x:38586,y:31065,ptovrint:False,ptlb:EmissionPINK,ptin:_EmissionPINK,varname:node_1662,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0,c3:0.475862,c4:1;n:type:ShaderForge.SFN_Lerp,id:9648,x:39263,y:31328,varname:node_9648,prsc:2|A-870-OUT,B-9538-RGB,T-720-OUT;n:type:ShaderForge.SFN_Color,id:9538,x:38967,y:31419,ptovrint:False,ptlb:EmissionGREEN,ptin:_EmissionGREEN,varname:node_9538,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.1323529,c2:1,c3:0.1742392,c4:1;n:type:ShaderForge.SFN_Lerp,id:7698,x:39704,y:31571,varname:node_7698,prsc:2|A-9648-OUT,B-2675-RGB,T-3325-OUT;n:type:ShaderForge.SFN_Color,id:2675,x:39404,y:31647,ptovrint:False,ptlb:EmissionYELLOW,ptin:_EmissionYELLOW,varname:node_2675,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0.8482758,c3:0,c4:1;n:type:ShaderForge.SFN_Lerp,id:8293,x:40194,y:31323,varname:node_8293,prsc:2|A-7698-OUT,B-2514-RGB,T-5371-OUT;n:type:ShaderForge.SFN_Color,id:2514,x:40188,y:31584,ptovrint:False,ptlb:EmissionRED,ptin:_EmissionRED,varname:node_2514,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0,c3:0,c4:1;proporder:1555-2932-1771-216-6596-3892-9258-9117-4785-9783-1632-7486-456-5060-1475-1010-7542-4062-2206-6867-5627-528-6604-1662-6612-4729-8637-341-2426-9538-9299-5521-1380-5832-9808-2675-1421-5700-8088-3749-1888-2514-8748-2851-720-3325-5371;pass:END;sub:END;*/

Shader "Assets/Cubes/Materials/Lava_shader" {
    Properties {
        _T_Lava01 ("T_Lava01", 2D) = "white" {}
        _T_Lava02 ("T_Lava02", 2D) = "white" {}
        _T_Lava03 ("T_Lava03", 2D) = "white" {}
        _T_Lava04 ("T_Lava04", 2D) = "white" {}
        _T_Lava05 ("T_Lava05", 2D) = "white" {}
        _T_Lavalpha02 ("T_Lavalpha02", 2D) = "white" {}
        _T_Lavalpha03 ("T_Lavalpha03", 2D) = "white" {}
        _T_Lavalpha04 ("T_Lavalpha04", 2D) = "white" {}
        _T_Lavalpha05 ("T_Lavalpha05", 2D) = "white" {}
        _Opacity ("Opacity", Range(0, 1)) = 1
        _LavaNEUTRAL ("LavaNEUTRAL", Color) = (0.5,0.5,0.5,1)
        _EmissionNEUTRAL ("EmissionNEUTRAL", Color) = (1,1,1,1)
        _LavaBLUE1 ("LavaBLUE1", Color) = (0.25,0.8758624,1,1)
        _LavaBLUE2 ("LavaBLUE2", Color) = (0.3235294,0.8880324,1,1)
        _LavaBLUE3 ("LavaBLUE3", Color) = (0,0.4206896,1,1)
        _LavaBLUE4 ("LavaBLUE4", Color) = (0,0.5862069,1,1)
        _LavaBLUE5 ("LavaBLUE5", Color) = (0,0.3793104,1,1)
        _EmmisionBLUE ("EmmisionBLUE", Color) = (0,0.9172413,1,1)
        _LavaPINK1 ("LavaPINK1", Color) = (1,0,0.7241378,1)
        _LavaPINK2 ("LavaPINK2", Color) = (1,0,0.3103447,1)
        _LavaPINK3 ("LavaPINK3", Color) = (1,0.1691176,0.4269776,1)
        _LavaPINK4 ("LavaPINK4", Color) = (1,0,0.3517241,1)
        _LavaPINK5 ("LavaPINK5", Color) = (1,0,0.3103447,1)
        _EmissionPINK ("EmissionPINK", Color) = (1,0,0.475862,1)
        _LavaGREEN1 ("LavaGREEN1", Color) = (0.3068966,1,0.25,1)
        _LavaGREEN2 ("LavaGREEN2", Color) = (0,0.6985294,0.09153153,1)
        _LavaGREEN3 ("LavaGREEN3", Color) = (0.03448272,1,0,1)
        _LavaGREEN4 ("LavaGREEN4", Color) = (0.04629824,0.6102941,0,1)
        _LavaGREEN5 ("LavaGREEN5", Color) = (0,0.8676471,0.2213995,1)
        _EmissionGREEN ("EmissionGREEN", Color) = (0.1323529,1,0.1742392,1)
        _LavaYELLOW1 ("LavaYELLOW1", Color) = (1,0.7241379,0,1)
        _LavaYELLOW2 ("LavaYELLOW2", Color) = (1,0.8068966,0,1)
        _LavaYELLOW3 ("LavaYELLOW3", Color) = (1,0.6,0,1)
        _LavaYELLOW4 ("LavaYELLOW4", Color) = (1,0.8482759,0,1)
        _LavaYELLOW5 ("LavaYELLOW5", Color) = (1,0.7655172,0,1)
        _EmissionYELLOW ("EmissionYELLOW", Color) = (1,0.8482758,0,1)
        _LavaRED1 ("LavaRED1", Color) = (1,0.2132353,0.2132353,1)
        _LavaRED2 ("LavaRED2", Color) = (1,0,0,1)
        _LavaRED3 ("LavaRED3", Color) = (0.6764706,0,0,1)
        _LavaRED4 ("LavaRED4", Color) = (0.8455882,0.08082826,0.08082826,1)
        _LavaRED5 ("LavaRED5", Color) = (0.5955882,0,0,1)
        _EmissionRED ("EmissionRED", Color) = (1,0,0,1)
        _LerpBLUE ("LerpBLUE", Range(0, 1)) = 0
        _LerpPINK ("LerpPINK", Range(0, 1)) = 0
        _LerpGREEN ("LerpGREEN", Range(0, 1)) = 0
        _LerpYELLOW ("LerpYELLOW", Range(0, 1)) = 0
        _LerpRED ("LerpRED", Range(0, 1)) = 0
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
            Blend One OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles n3ds wiiu 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _TimeEditor;
            uniform sampler2D _T_Lava01; uniform float4 _T_Lava01_ST;
            uniform sampler2D _T_Lava02; uniform float4 _T_Lava02_ST;
            uniform sampler2D _T_Lavalpha02; uniform float4 _T_Lavalpha02_ST;
            uniform sampler2D _T_Lava03; uniform float4 _T_Lava03_ST;
            uniform sampler2D _T_Lavalpha03; uniform float4 _T_Lavalpha03_ST;
            uniform sampler2D _T_Lava04; uniform float4 _T_Lava04_ST;
            uniform sampler2D _T_Lavalpha04; uniform float4 _T_Lavalpha04_ST;
            uniform sampler2D _T_Lava05; uniform float4 _T_Lava05_ST;
            uniform sampler2D _T_Lavalpha05; uniform float4 _T_Lavalpha05_ST;
            uniform float4 _LavaBLUE1;
            uniform float4 _LavaBLUE2;
            uniform float4 _LavaBLUE3;
            uniform float4 _LavaBLUE4;
            uniform float4 _LavaBLUE5;
            uniform float4 _EmmisionBLUE;
            uniform float4 _LavaNEUTRAL;
            uniform float _LerpBLUE;
            uniform float4 _EmissionNEUTRAL;
            uniform float _Opacity;
            uniform float4 _LavaGREEN1;
            uniform float4 _LavaGREEN2;
            uniform float4 _LavaGREEN3;
            uniform float4 _LavaGREEN4;
            uniform float4 _LavaGREEN5;
            uniform float4 _LavaPINK5;
            uniform float4 _LavaYELLOW5;
            uniform float4 _LavaRED5;
            uniform float _LerpGREEN;
            uniform float _LerpPINK;
            uniform float _LerpYELLOW;
            uniform float _LerpRED;
            uniform float4 _LavaPINK1;
            uniform float4 _LavaPINK2;
            uniform float4 _LavaPINK3;
            uniform float4 _LavaPINK4;
            uniform float4 _LavaYELLOW1;
            uniform float4 _LavaYELLOW2;
            uniform float4 _LavaYELLOW3;
            uniform float4 _LavaYELLOW4;
            uniform float4 _LavaRED1;
            uniform float4 _LavaRED2;
            uniform float4 _LavaRED3;
            uniform float4 _LavaRED4;
            uniform float4 _EmissionPINK;
            uniform float4 _EmissionGREEN;
            uniform float4 _EmissionYELLOW;
            uniform float4 _EmissionRED;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                UNITY_FOG_COORDS(3)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float4 node_5665 = _Time + _TimeEditor;
                float2 node_3515 = (i.uv0+node_5665.g*float2(0,0.1));
                float4 _T_Lava01_var = tex2D(_T_Lava01,TRANSFORM_TEX(node_3515, _T_Lava01));
                float2 node_5482 = (i.uv0+node_5665.g*float2(0.5,0.3));
                float4 _T_Lava02_var = tex2D(_T_Lava02,TRANSFORM_TEX(node_5482, _T_Lava02));
                float4 _T_Lavalpha02_var = tex2D(_T_Lavalpha02,TRANSFORM_TEX(node_5482, _T_Lavalpha02));
                float2 node_4922 = (i.uv0+node_5665.g*float2(0,0.2));
                float4 _T_Lava03_var = tex2D(_T_Lava03,TRANSFORM_TEX(node_4922, _T_Lava03));
                float4 _T_Lavalpha03_var = tex2D(_T_Lavalpha03,TRANSFORM_TEX(node_4922, _T_Lavalpha03));
                float2 node_3039 = (i.uv0+node_5665.g*float2(1,0.5));
                float4 _T_Lava04_var = tex2D(_T_Lava04,TRANSFORM_TEX(node_3039, _T_Lava04));
                float4 _T_Lavalpha04_var = tex2D(_T_Lavalpha04,TRANSFORM_TEX(node_3039, _T_Lavalpha04));
                float2 node_3076 = (i.uv0+node_5665.g*float2(0,0.4));
                float4 _T_Lava05_var = tex2D(_T_Lava05,TRANSFORM_TEX(node_3076, _T_Lava05));
                float4 _T_Lavalpha05_var = tex2D(_T_Lavalpha05,TRANSFORM_TEX(node_3076, _T_Lavalpha05));
                float3 node_3274 = lerp(lerp(lerp(lerp((_LavaNEUTRAL.rgb+_T_Lava01_var.rgb),(_LavaNEUTRAL.rgb+_T_Lava02_var.rgb),_T_Lavalpha02_var.rgb),(_LavaNEUTRAL.rgb+_T_Lava03_var.rgb),_T_Lavalpha03_var.rgb),(_LavaNEUTRAL.rgb+_T_Lava04_var.rgb),_T_Lavalpha04_var.rgb),(_LavaNEUTRAL.rgb+_T_Lava05_var.rgb),_T_Lavalpha05_var.rgb);
                float3 diffuseColor = lerp(lerp(lerp(lerp(lerp(node_3274,lerp(lerp(lerp(lerp((_LavaBLUE1.rgb+_T_Lava01_var.rgb),(_LavaBLUE2.rgb+_T_Lava02_var.rgb),_T_Lavalpha02_var.rgb),(_LavaBLUE3.rgb+_T_Lava03_var.rgb),_T_Lavalpha03_var.rgb),(_LavaBLUE4.rgb+_T_Lava04_var.rgb),_T_Lavalpha04_var.rgb),(_LavaBLUE5.rgb+_T_Lava05_var.rgb),_T_Lavalpha05_var.rgb),_LerpBLUE),lerp(node_3274,lerp(lerp(lerp(lerp((_LavaPINK1.rgb+_T_Lava01_var.rgb),(_LavaPINK2.rgb+_T_Lava02_var.rgb),_T_Lavalpha02_var.rgb),(_LavaPINK3.rgb+_T_Lava03_var.rgb),_T_Lavalpha03_var.rgb),(_LavaPINK4.rgb+_T_Lava04_var.rgb),_T_Lavalpha04_var.rgb),(_LavaPINK5.rgb+_T_Lava05_var.rgb),_T_Lavalpha05_var.rgb),_LerpPINK),_LerpPINK),lerp(node_3274,lerp(lerp(lerp(lerp((_LavaGREEN1.rgb+_T_Lava01_var.rgb),(_LavaGREEN2.rgb+_T_Lava02_var.rgb),_T_Lavalpha02_var.rgb),(_LavaGREEN3.rgb+_T_Lava03_var.rgb),_T_Lavalpha03_var.rgb),(_LavaGREEN4.rgb+_T_Lava04_var.rgb),_T_Lavalpha04_var.rgb),(_LavaGREEN5.rgb+_T_Lava05_var.rgb),_T_Lavalpha05_var.rgb),_LerpGREEN),_LerpGREEN),lerp(node_3274,lerp(lerp(lerp(lerp((_LavaYELLOW1.rgb+_T_Lava01_var.rgb),(_LavaYELLOW2.rgb+_T_Lava02_var.rgb),_T_Lavalpha02_var.rgb),(_LavaYELLOW3.rgb+_T_Lava03_var.rgb),_T_Lavalpha03_var.rgb),(_LavaYELLOW4.rgb+_T_Lava04_var.rgb),_T_Lavalpha04_var.rgb),(_LavaYELLOW5.rgb+_T_Lava05_var.rgb),_T_Lavalpha05_var.rgb),_LerpYELLOW),_LerpYELLOW),lerp(lerp(lerp(lerp((_LavaRED1.rgb+_T_Lava01_var.rgb),(_LavaRED2.rgb+_T_Lava02_var.rgb),_T_Lavalpha02_var.rgb),(_LavaRED3.rgb+_T_Lava03_var.rgb),_T_Lavalpha03_var.rgb),(_LavaRED4.rgb+_T_Lava04_var.rgb),_T_Lavalpha04_var.rgb),(_LavaRED5.rgb+_T_Lava05_var.rgb),_T_Lavalpha05_var.rgb),_LerpRED);
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                float3 emissive = lerp(lerp(lerp(lerp(lerp(_EmissionNEUTRAL.rgb,_EmmisionBLUE.rgb,_LerpBLUE),_EmissionPINK.rgb,_LerpPINK),_EmissionGREEN.rgb,_LerpGREEN),_EmissionYELLOW.rgb,_LerpYELLOW),_EmissionRED.rgb,_LerpRED);
/// Final Color:
                float3 finalColor = diffuse * _Opacity + emissive;
                fixed4 finalRGBA = fixed4(finalColor,_Opacity);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles n3ds wiiu 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _TimeEditor;
            uniform sampler2D _T_Lava01; uniform float4 _T_Lava01_ST;
            uniform sampler2D _T_Lava02; uniform float4 _T_Lava02_ST;
            uniform sampler2D _T_Lavalpha02; uniform float4 _T_Lavalpha02_ST;
            uniform sampler2D _T_Lava03; uniform float4 _T_Lava03_ST;
            uniform sampler2D _T_Lavalpha03; uniform float4 _T_Lavalpha03_ST;
            uniform sampler2D _T_Lava04; uniform float4 _T_Lava04_ST;
            uniform sampler2D _T_Lavalpha04; uniform float4 _T_Lavalpha04_ST;
            uniform sampler2D _T_Lava05; uniform float4 _T_Lava05_ST;
            uniform sampler2D _T_Lavalpha05; uniform float4 _T_Lavalpha05_ST;
            uniform float4 _LavaBLUE1;
            uniform float4 _LavaBLUE2;
            uniform float4 _LavaBLUE3;
            uniform float4 _LavaBLUE4;
            uniform float4 _LavaBLUE5;
            uniform float4 _EmmisionBLUE;
            uniform float4 _LavaNEUTRAL;
            uniform float _LerpBLUE;
            uniform float4 _EmissionNEUTRAL;
            uniform float _Opacity;
            uniform float4 _LavaGREEN1;
            uniform float4 _LavaGREEN2;
            uniform float4 _LavaGREEN3;
            uniform float4 _LavaGREEN4;
            uniform float4 _LavaGREEN5;
            uniform float4 _LavaPINK5;
            uniform float4 _LavaYELLOW5;
            uniform float4 _LavaRED5;
            uniform float _LerpGREEN;
            uniform float _LerpPINK;
            uniform float _LerpYELLOW;
            uniform float _LerpRED;
            uniform float4 _LavaPINK1;
            uniform float4 _LavaPINK2;
            uniform float4 _LavaPINK3;
            uniform float4 _LavaPINK4;
            uniform float4 _LavaYELLOW1;
            uniform float4 _LavaYELLOW2;
            uniform float4 _LavaYELLOW3;
            uniform float4 _LavaYELLOW4;
            uniform float4 _LavaRED1;
            uniform float4 _LavaRED2;
            uniform float4 _LavaRED3;
            uniform float4 _LavaRED4;
            uniform float4 _EmissionPINK;
            uniform float4 _EmissionGREEN;
            uniform float4 _EmissionYELLOW;
            uniform float4 _EmissionRED;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float4 node_3559 = _Time + _TimeEditor;
                float2 node_3515 = (i.uv0+node_3559.g*float2(0,0.1));
                float4 _T_Lava01_var = tex2D(_T_Lava01,TRANSFORM_TEX(node_3515, _T_Lava01));
                float2 node_5482 = (i.uv0+node_3559.g*float2(0.5,0.3));
                float4 _T_Lava02_var = tex2D(_T_Lava02,TRANSFORM_TEX(node_5482, _T_Lava02));
                float4 _T_Lavalpha02_var = tex2D(_T_Lavalpha02,TRANSFORM_TEX(node_5482, _T_Lavalpha02));
                float2 node_4922 = (i.uv0+node_3559.g*float2(0,0.2));
                float4 _T_Lava03_var = tex2D(_T_Lava03,TRANSFORM_TEX(node_4922, _T_Lava03));
                float4 _T_Lavalpha03_var = tex2D(_T_Lavalpha03,TRANSFORM_TEX(node_4922, _T_Lavalpha03));
                float2 node_3039 = (i.uv0+node_3559.g*float2(1,0.5));
                float4 _T_Lava04_var = tex2D(_T_Lava04,TRANSFORM_TEX(node_3039, _T_Lava04));
                float4 _T_Lavalpha04_var = tex2D(_T_Lavalpha04,TRANSFORM_TEX(node_3039, _T_Lavalpha04));
                float2 node_3076 = (i.uv0+node_3559.g*float2(0,0.4));
                float4 _T_Lava05_var = tex2D(_T_Lava05,TRANSFORM_TEX(node_3076, _T_Lava05));
                float4 _T_Lavalpha05_var = tex2D(_T_Lavalpha05,TRANSFORM_TEX(node_3076, _T_Lavalpha05));
                float3 node_3274 = lerp(lerp(lerp(lerp((_LavaNEUTRAL.rgb+_T_Lava01_var.rgb),(_LavaNEUTRAL.rgb+_T_Lava02_var.rgb),_T_Lavalpha02_var.rgb),(_LavaNEUTRAL.rgb+_T_Lava03_var.rgb),_T_Lavalpha03_var.rgb),(_LavaNEUTRAL.rgb+_T_Lava04_var.rgb),_T_Lavalpha04_var.rgb),(_LavaNEUTRAL.rgb+_T_Lava05_var.rgb),_T_Lavalpha05_var.rgb);
                float3 diffuseColor = lerp(lerp(lerp(lerp(lerp(node_3274,lerp(lerp(lerp(lerp((_LavaBLUE1.rgb+_T_Lava01_var.rgb),(_LavaBLUE2.rgb+_T_Lava02_var.rgb),_T_Lavalpha02_var.rgb),(_LavaBLUE3.rgb+_T_Lava03_var.rgb),_T_Lavalpha03_var.rgb),(_LavaBLUE4.rgb+_T_Lava04_var.rgb),_T_Lavalpha04_var.rgb),(_LavaBLUE5.rgb+_T_Lava05_var.rgb),_T_Lavalpha05_var.rgb),_LerpBLUE),lerp(node_3274,lerp(lerp(lerp(lerp((_LavaPINK1.rgb+_T_Lava01_var.rgb),(_LavaPINK2.rgb+_T_Lava02_var.rgb),_T_Lavalpha02_var.rgb),(_LavaPINK3.rgb+_T_Lava03_var.rgb),_T_Lavalpha03_var.rgb),(_LavaPINK4.rgb+_T_Lava04_var.rgb),_T_Lavalpha04_var.rgb),(_LavaPINK5.rgb+_T_Lava05_var.rgb),_T_Lavalpha05_var.rgb),_LerpPINK),_LerpPINK),lerp(node_3274,lerp(lerp(lerp(lerp((_LavaGREEN1.rgb+_T_Lava01_var.rgb),(_LavaGREEN2.rgb+_T_Lava02_var.rgb),_T_Lavalpha02_var.rgb),(_LavaGREEN3.rgb+_T_Lava03_var.rgb),_T_Lavalpha03_var.rgb),(_LavaGREEN4.rgb+_T_Lava04_var.rgb),_T_Lavalpha04_var.rgb),(_LavaGREEN5.rgb+_T_Lava05_var.rgb),_T_Lavalpha05_var.rgb),_LerpGREEN),_LerpGREEN),lerp(node_3274,lerp(lerp(lerp(lerp((_LavaYELLOW1.rgb+_T_Lava01_var.rgb),(_LavaYELLOW2.rgb+_T_Lava02_var.rgb),_T_Lavalpha02_var.rgb),(_LavaYELLOW3.rgb+_T_Lava03_var.rgb),_T_Lavalpha03_var.rgb),(_LavaYELLOW4.rgb+_T_Lava04_var.rgb),_T_Lavalpha04_var.rgb),(_LavaYELLOW5.rgb+_T_Lava05_var.rgb),_T_Lavalpha05_var.rgb),_LerpYELLOW),_LerpYELLOW),lerp(lerp(lerp(lerp((_LavaRED1.rgb+_T_Lava01_var.rgb),(_LavaRED2.rgb+_T_Lava02_var.rgb),_T_Lavalpha02_var.rgb),(_LavaRED3.rgb+_T_Lava03_var.rgb),_T_Lavalpha03_var.rgb),(_LavaRED4.rgb+_T_Lava04_var.rgb),_T_Lavalpha04_var.rgb),(_LavaRED5.rgb+_T_Lava05_var.rgb),_T_Lavalpha05_var.rgb),_LerpRED);
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse * _Opacity;
                fixed4 finalRGBA = fixed4(finalColor,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
