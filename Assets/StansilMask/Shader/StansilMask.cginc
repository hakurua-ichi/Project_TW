#ifndef WALL_STENCIL_OPERATIONS_INCLUDED
#define WALL_STENCIL_OPERATIONS_INCLUDED

// 벽이 투명해져야 할 때 적용할 스텐실 설정
#define WALL_TRANSPARENT_STENCIL_MASK \
Stencil \
{ \
    Ref 2 \
    Comp Equal \
    Pass Keep \
    ReadMask 255 \
    WriteMask 0 \
}

#endif