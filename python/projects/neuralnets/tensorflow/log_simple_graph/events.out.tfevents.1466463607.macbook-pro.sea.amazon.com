       �K"	  �]��Abrain.Event:2�#F�      A�	@	���]��A"�?
J
inputConst*
dtype0*
valueB
 *  �?*
_output_shapes
: 
Y
weight/initial_valueConst*
dtype0*
valueB
 *��L?*
_output_shapes
: 
h
weightVariable*
dtype0*
shared_name *
_output_shapes
: *
	container *
shape: 
�
weight/AssignAssignweightweight/initial_value*
_output_shapes
: *
T0*
_class
loc:@weight*
validate_shape(*
use_locking(
[
weight/readIdentityweight*
_output_shapes
: *
_class
loc:@weight*
T0
B
outputMulinputweight/read*
_output_shapes
: *
T0
R
correct_valueConst*
dtype0*
valueB
 *    *
_output_shapes
: 
B
subSuboutputcorrect_value*
_output_shapes
: *
T0
K
loss/yConst*
dtype0*
valueB
 *   @*
_output_shapes
: 
9
lossPowsubloss/y*
_output_shapes
: *
T0
A
gradients/ShapeShapeloss*
_output_shapes
: *
T0
T
gradients/ConstConst*
dtype0*
valueB
 *  �?*
_output_shapes
: 
Y
gradients/FillFillgradients/Shapegradients/Const*
_output_shapes
: *
T0
J
gradients/loss_grad/ShapeShapesub*
_output_shapes
: *
T0
O
gradients/loss_grad/Shape_1Shapeloss/y*
_output_shapes
: *
T0
�
)gradients/loss_grad/BroadcastGradientArgsBroadcastGradientArgsgradients/loss_grad/Shapegradients/loss_grad/Shape_1*2
_output_shapes 
:���������:���������
W
gradients/loss_grad/mulMulgradients/Fillloss/y*
_output_shapes
: *
T0
^
gradients/loss_grad/sub/yConst*
dtype0*
valueB
 *  �?*
_output_shapes
: 
b
gradients/loss_grad/subSubloss/ygradients/loss_grad/sub/y*
_output_shapes
: *
T0
]
gradients/loss_grad/PowPowsubgradients/loss_grad/sub*
_output_shapes
: *
T0
s
gradients/loss_grad/mul_1Mulgradients/loss_grad/mulgradients/loss_grad/Pow*
_output_shapes
: *
T0
�
gradients/loss_grad/SumSumgradients/loss_grad/mul_1)gradients/loss_grad/BroadcastGradientArgs*
_output_shapes
:*
	keep_dims( *
T0
{
gradients/loss_grad/ReshapeReshapegradients/loss_grad/Sumgradients/loss_grad/Shape*
_output_shapes
: *
T0
W
gradients/loss_grad/mul_2Mulgradients/Fillloss*
_output_shapes
: *
T0
D
gradients/loss_grad/LogLogsub*
_output_shapes
: *
T0
u
gradients/loss_grad/mul_3Mulgradients/loss_grad/mul_2gradients/loss_grad/Log*
_output_shapes
: *
T0
�
gradients/loss_grad/Sum_1Sumgradients/loss_grad/mul_3+gradients/loss_grad/BroadcastGradientArgs:1*
_output_shapes
:*
	keep_dims( *
T0
�
gradients/loss_grad/Reshape_1Reshapegradients/loss_grad/Sum_1gradients/loss_grad/Shape_1*
_output_shapes
: *
T0
j
$gradients/loss_grad/tuple/group_depsNoOp^gradients/loss_grad/Reshape^gradients/loss_grad/Reshape_1
�
,gradients/loss_grad/tuple/control_dependencyIdentitygradients/loss_grad/Reshape%^gradients/loss_grad/tuple/group_deps*
_output_shapes
: *.
_class$
" loc:@gradients/loss_grad/Reshape*
T0
�
.gradients/loss_grad/tuple/control_dependency_1Identitygradients/loss_grad/Reshape_1%^gradients/loss_grad/tuple/group_deps*
_output_shapes
: *0
_class&
$"loc:@gradients/loss_grad/Reshape_1*
T0
L
gradients/sub_grad/ShapeShapeoutput*
_output_shapes
: *
T0
U
gradients/sub_grad/Shape_1Shapecorrect_value*
_output_shapes
: *
T0
�
(gradients/sub_grad/BroadcastGradientArgsBroadcastGradientArgsgradients/sub_grad/Shapegradients/sub_grad/Shape_1*2
_output_shapes 
:���������:���������
�
gradients/sub_grad/SumSum,gradients/loss_grad/tuple/control_dependency(gradients/sub_grad/BroadcastGradientArgs*
_output_shapes
:*
	keep_dims( *
T0
x
gradients/sub_grad/ReshapeReshapegradients/sub_grad/Sumgradients/sub_grad/Shape*
_output_shapes
: *
T0
�
gradients/sub_grad/Sum_1Sum,gradients/loss_grad/tuple/control_dependency*gradients/sub_grad/BroadcastGradientArgs:1*
_output_shapes
:*
	keep_dims( *
T0
Z
gradients/sub_grad/NegNeggradients/sub_grad/Sum_1*
_output_shapes
:*
T0
|
gradients/sub_grad/Reshape_1Reshapegradients/sub_grad/Neggradients/sub_grad/Shape_1*
_output_shapes
: *
T0
g
#gradients/sub_grad/tuple/group_depsNoOp^gradients/sub_grad/Reshape^gradients/sub_grad/Reshape_1
�
+gradients/sub_grad/tuple/control_dependencyIdentitygradients/sub_grad/Reshape$^gradients/sub_grad/tuple/group_deps*
_output_shapes
: *-
_class#
!loc:@gradients/sub_grad/Reshape*
T0
�
-gradients/sub_grad/tuple/control_dependency_1Identitygradients/sub_grad/Reshape_1$^gradients/sub_grad/tuple/group_deps*
_output_shapes
: */
_class%
#!loc:@gradients/sub_grad/Reshape_1*
T0
N
gradients/output_grad/ShapeShapeinput*
_output_shapes
: *
T0
V
gradients/output_grad/Shape_1Shapeweight/read*
_output_shapes
: *
T0
�
+gradients/output_grad/BroadcastGradientArgsBroadcastGradientArgsgradients/output_grad/Shapegradients/output_grad/Shape_1*2
_output_shapes 
:���������:���������
{
gradients/output_grad/mulMul+gradients/sub_grad/tuple/control_dependencyweight/read*
_output_shapes
: *
T0
�
gradients/output_grad/SumSumgradients/output_grad/mul+gradients/output_grad/BroadcastGradientArgs*
_output_shapes
:*
	keep_dims( *
T0
�
gradients/output_grad/ReshapeReshapegradients/output_grad/Sumgradients/output_grad/Shape*
_output_shapes
: *
T0
w
gradients/output_grad/mul_1Mulinput+gradients/sub_grad/tuple/control_dependency*
_output_shapes
: *
T0
�
gradients/output_grad/Sum_1Sumgradients/output_grad/mul_1-gradients/output_grad/BroadcastGradientArgs:1*
_output_shapes
:*
	keep_dims( *
T0
�
gradients/output_grad/Reshape_1Reshapegradients/output_grad/Sum_1gradients/output_grad/Shape_1*
_output_shapes
: *
T0
p
&gradients/output_grad/tuple/group_depsNoOp^gradients/output_grad/Reshape ^gradients/output_grad/Reshape_1
�
.gradients/output_grad/tuple/control_dependencyIdentitygradients/output_grad/Reshape'^gradients/output_grad/tuple/group_deps*
_output_shapes
: *0
_class&
$"loc:@gradients/output_grad/Reshape*
T0
�
0gradients/output_grad/tuple/control_dependency_1Identitygradients/output_grad/Reshape_1'^gradients/output_grad/tuple/group_deps*
_output_shapes
: *2
_class(
&$loc:@gradients/output_grad/Reshape_1*
T0
b
GradientDescent/learning_rateConst*
dtype0*
valueB
 *���<*
_output_shapes
: 
�
2GradientDescent/update_weight/ApplyGradientDescentApplyGradientDescentweightGradientDescent/learning_rate0gradients/output_grad/tuple/control_dependency_1*
_output_shapes
: *
T0*
_class
loc:@weight*
use_locking( 
L
GradientDescentNoOp3^GradientDescent/update_weight/ApplyGradientDescent
X
ScalarSummary/tagsConst*
dtype0*
valueB Binput*
_output_shapes
: 
Z
ScalarSummaryScalarSummaryScalarSummary/tagsinput*
_output_shapes
: *
T0
[
ScalarSummary_1/tagsConst*
dtype0*
valueB Bweight*
_output_shapes
: 
d
ScalarSummary_1ScalarSummaryScalarSummary_1/tagsweight/read*
_output_shapes
: *
T0
[
ScalarSummary_2/tagsConst*
dtype0*
valueB Boutput*
_output_shapes
: 
_
ScalarSummary_2ScalarSummaryScalarSummary_2/tagsoutput*
_output_shapes
: *
T0
b
ScalarSummary_3/tagsConst*
dtype0*
valueB Bcorrect_value*
_output_shapes
: 
f
ScalarSummary_3ScalarSummaryScalarSummary_3/tagscorrect_value*
_output_shapes
: *
T0
Y
ScalarSummary_4/tagsConst*
dtype0*
valueB
 Bloss*
_output_shapes
: 
]
ScalarSummary_4ScalarSummaryScalarSummary_4/tagsloss*
_output_shapes
: *
T0
�
MergeSummary/MergeSummaryMergeSummaryScalarSummaryScalarSummary_1ScalarSummary_2ScalarSummary_3ScalarSummary_4*
_output_shapes
: *
N"	+�ZTZ       o��	���]��A*O

input  �?

weight��L?

output��L?

correct_value    

loss�#?�� �\       ����	���]��A*O

input  �?

weight\�B?

output\�B?

correct_value    

loss��?���\       ����	���]��A*O

input  �?

weight��8?

output��8?

correct_value    

loss�r?x�O�\       ����	_��]��A*O

input  �?

weight$�/?

output$�/?

correct_value    

loss���>k�\       ����	o��]��A*O

input  �?

weight��&?

output��&?

correct_value    

loss�c�>~�=�\       ����	���]��A*O

input  �?

weightgx?

outputgx?

correct_value    

loss�1�>B��\       ����	�]��A*O

input  �?

weight��?

output��?

correct_value    

loss��>��~\       ����	��]��A*O

input  �?

weight�?

output�?

correct_value    

loss!͟>x��\       ����	�]��A*O

input  �?

weightU�?

outputU�?

correct_value    

loss}8�>(Y�1\       ����	�$�]��A	*O

input  �?

weight7?

output7?

correct_value    

loss�(�>��w�\       ����	�-�]��A
*O

input  �?

weight>�>

output>�>

correct_value    

loss��j>��0\       ����		7�]��A*O

input  �?

weight��>

output��>

correct_value    

loss�T>.�UJ\       ����	1@�]��A*O

input  �?

weight�T�>

output�T�>

correct_value    

loss�[?>ﮖ�\       ����	4I�]��A*O

input  �?

weight�C�>

output�C�>

correct_value    

lossS�,>��^:\       ����	iR�]��A*O

input  �?

weightl��>

outputl��>

correct_value    

loss��>��\       ����	[�]��A*O

input  �?

weight�ý>

output�ý>

correct_value    

lossg�>����\       ����	9d�]��A*O

input  �?

weight�F�>

output�F�>

correct_value    

loss���=ۉi�\       ����	"m�]��A*O

input  �?

weightC�>

outputC�>

correct_value    

losse%�=�Wp`\       ����	v�]��A*O

input  �?

weightﲢ>

outputﲢ>

correct_value    

loss���=Y���\       ����	�~�]��A*O

input  �?

weightc��>

outputc��>

correct_value    

loss��=]��\       ����	U��]��A*O

input  �?

weight�Ւ>

output�Ւ>

correct_value    

loss�q�=�R��\       ����	��]��A*O

input  �?

weightx~�>

outputx~�>

correct_value    

loss*�=���X\       ����	��]��A*O

input  �?

weight�>

output�>

correct_value    

loss�2�=H�\       ����	���]��A*O

input  �?

weighte�{>

outpute�{>

correct_value    

loss��w=_!W\       ����	K��]��A*O

input  �?

weight�2o>

output�2o>

correct_value    

loss`_=�q�6\       ����	g��]��A*O

input  �?

weight�<c>

output�<c>

correct_value    

loss�I=�1��\       ����	a��]��A*O

input  �?

weight(�W>

output(�W>

correct_value    

lossG
6=��`�\       ����	���]��A*O

input  �?

weight�M>

output�M>

correct_value    

loss�J$=�n)B\       ����	A��]��A*O

input  �?

weight��B>

output��B>

correct_value    

loss�E=���.\       ����	���]��A*O

input  �?

weight9>

output9>

correct_value    

loss��=WP��\       ����	!��]��A*O

input  �?

weight �/>

output �/>

correct_value    

lossΉ�<�\       ����	���]��A*O

input  �?

weightZ
'>

outputZ
'>

correct_value    

loss��<��qg\       ����	���]��A *O

input  �?

weight<�>

output<�>

correct_value    

loss��<���{\       ����	"��]��A!*O

input  �?

weight�>

output�>

correct_value    

loss���<��\       ����	�]��A"*O

input  �?

weight_7>

output_7>

correct_value    

loss�=�<�O{\       ����	��]��A#*O

input  �?

weight4>

output4>

correct_value    

loss0��<��n\       ����	��]��A$*O

input  �?

weight�@>

output�@>

correct_value    

loss���<�\\       ����	�!�]��A%*O

input  �?

weight���=

output���=

correct_value    

loss��k<�s_�\       ����	�*�]��A&*O

input  �?

weightM�=

outputM�=

correct_value    

lossk�T<�v�\       ����	/3�]��A'*O

input  �?

weight֢�=

output֢�=

correct_value    

loss��?<^�\       ����	�;�]��A(*O

input  �?

weight��=

output��=

correct_value    

loss--<S�s�\       ����	9F�]��A)*O

input  �?

weight��=

output��=

correct_value    

loss�J<��^�\       ����	O�]��A**O

input  �?

weightv�=

outputv�=

correct_value    

loss�<��m�\       ����	�W�]��A+*O

input  �?

weight#��=

output#��=

correct_value    

lossϙ�;���\       ����	t`�]��A,*O

input  �?

weightn�=

outputn�=

correct_value    

loss���;����\       ����	i�]��A-*O

input  �?

weightB�=

outputB�=

correct_value    

loss�_�;�m\       ����	�q�]��A.*O

input  �?

weight�ƚ=

output�ƚ=

correct_value    

loss�'�;��˼\       ����	z�]��A/*O

input  �?

weight�	�=

output�	�=

correct_value    

lossJ�;~4�M\       ����	˂�]��A0*O

input  �?

weight���=

output���=

correct_value    

loss\p�;5~��\       ����	3��]��A1*O

input  �?

weight���=

output���=

correct_value    

loss|��;���b\       ����	���]��A2*O

input  �?

weight"|=

output"|=

correct_value    

loss,Sx;/���\       ����	o��]��A3*O

input  �?

weightΆo=

outputΆo=

correct_value    

loss�`;$͋\       ����	z��]��A4*O

input  �?

weight݌c=

output݌c=

correct_value    

lossCJ;�Z\       ����	���]��A5*O

input  �?

weight8,X=

output8,X=

correct_value    

loss��6;��\       ����	��]��A6*O

input  �?

weight5]M=

output5]M=

correct_value    

lossi�$;�^)�\       ����	X��]��A7*O

input  �?

weight�C=

output�C=

correct_value    

lossh�;�"�\       ����	���]��A8*O

input  �?

weightRW9=

outputRW9=

correct_value    

lossR/;H��\       ����	���]��A9*O

input  �?

weight�0=

output�0=

correct_value    

loss"4�:v�¢\       ����	+��]��A:*O

input  �?

weight5E'=

output5E'=

correct_value    

loss���:eWka\       ����	���]��A;*O

input  �?

weight&�=

output&�=

correct_value    

loss�F�:��g\       ����	���]��A<*O

input  �?

weight$�=

output$�=

correct_value    

loss�
�:/��;\       ����	���]��A=*O

input  �?

weight�i=

output�i=

correct_value    

lossϮ�:�7ub\       ����	\�]��A>*O

input  �?

weight$>=

output$>=

correct_value    

loss+�:��r\       ����	��]��A?*O

input  �?

weight<n=

output<n=

correct_value    

loss���:���*\       ����	��]��A@*O

input  �?

weight��<

output��<

correct_value    

loss�;l: ~yC\       ����	q�]��AA*O

input  �?

weightK��<

outputK��<

correct_value    

lossZ3U:Z̴\       ����	(�]��AB*O

input  �?

weight���<

output���<

correct_value    

loss�i@:�U��\       ����	H1�]��AC*O

input  �?

weight��<

output��<

correct_value    

loss9�-:�fA\       ����	&:�]��AD*O

input  �?

weightGM�<

outputGM�<

correct_value    

lossָ:eR\       ����	�B�]��AE*O

input  �?

weightjI�<

outputjI�<

correct_value    

lossq:o���\       ����	QL�]��AF*O

input  �?

weight�Ŵ<

output�Ŵ<

correct_value    

lossXM�9~(�\       ����	GU�]��AG*O

input  �?

weightۻ�<

outputۻ�<

correct_value    

lossi�9��	�\       ����	�]�]��AH*O

input  �?

weight�%�<

output�%�<

correct_value    

loss���9��p\       ����	�f�]��AI*O

input  �?

weightb��<

outputb��<

correct_value    

loss���9f�C\       ����	�n�]��AJ*O

input  �?

weight�=�<

output�=�<

correct_value    

lossi_�9�G�<\       ����	�w�]��AK*O

input  �?

weight���<

output���<

correct_value    

loss�ۘ9���\       ����	
��]��AL*O

input  �?

weightf�<

outputf�<

correct_value    

loss��9���S\       ����	z��]��AM*O

input  �?

weight�z|<

output�z|<

correct_value    

lossMy9I�˱\       ����	��]��AN*O

input  �?

weight6�o<

output6�o<

correct_value    

loss�`9)�\       ����	Ú�]��AO*O

input  �?

weight�c<

output�c<

correct_value    

loss��J9oS�\       ����	v��]��AP*O

input  �?

weightfxX<

outputfxX<

correct_value    

losse79�T��\       ����	��]��AQ*O

input  �?

weight��M<

output��M<

correct_value    

loss�2%9�.�3\       ����	���]��AR*O

input  �?

weightM]C<

outputM]C<

correct_value    

lossE9�z�\       ����	���]��AS*O

input  �?

weight��9<

output��9<

correct_value    

loss��9�b)�\       ����	��]��AT*O

input  �?

weightQ0<

outputQ0<

correct_value    

loss���8U�JV\       ����	[��]��AU*O

input  �?

weight'�'<

output'�'<

correct_value    

loss�0�8�h��\       ����	��]��AV*O

input  �?

weight% <

output% <

correct_value    

loss���8��j\       ����	���]��AW*O

input  �?

weightV+<

outputV+<

correct_value    

lossN��8��\       ����	[��]��AX*O

input  �?

weight^�<

output^�<

correct_value    

loss! �8����\       ����	���]��AY*O

input  �?

weight&n<

output&n<

correct_value    

losspj�8֫փ\       ����		��]��AZ*O

input  �?

weightכ<

outputכ<

correct_value    

loss�<�8{κc\       ����	��]��A[*O

input  �?

weight�A�;

output�A�;

correct_value    

lossS�l8W�\       ����	k�]��A\*O

input  �?

weight���;

output���;

correct_value    

loss��U8��\       ����	v�]��A]*O

input  �?

weight!?�;

output!?�;

correct_value    

loss��@8��Lj\       ����	 �]��A^*O

input  �?

weight_"�;

output_"�;

correct_value    

loss�!.81��\       ����	�(�]��A_*O

input  �?

weightړ�;

outputړ�;

correct_value    

lossZ'8~��\       ����	Y2�]��A`*O

input  �?

weightv��;

outputv��;

correct_value    

loss��8�e�d\       ����	;�]��Aa*O

input  �?

weightp�;

outputp�;

correct_value    

loss�  8�$\       ����	|C�]��Ab*O

input  �?

weight^��;

output^��;

correct_value    

loss}�7��N\       ����	L�]��Ac*O

input  �?

weight&_�;

output&_�;

correct_value    

loss���7!�l