function T = hamming(S, encode=1)
%hamming Compute the hamming code for the specified input
%    S = the input vector to peform the hamming code on
%    This is the hamming (7,4) variation

% function constants
G = [1,0,0,0,1,0,1; 0,1,0,0,1,1,0; 0,0,1,0,1,1,1;0,0,0,1,0,1,1];
P = G'(5:7, :);
H = [P eye(3)];

% encoding function
if encode == 1,
  T = S * G;
else
  T = S * G;
end
% H * T = [0; 0; 0];
% r = (s * G) + n;
% H*n = z; % most likely n
end

