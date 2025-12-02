import React, { useRef, useState } from 'react';
import clsx from 'clsx';

type AvatarSize = 'xs' | 'sm' | 'md' | 'lg' | 'xl';
type Rounded = '0' | '1' | '2' | '3' | '4' | '5' | 'circle';

type AvatarProps = React.ComponentPropsWithoutRef<'div'> & {
  text: string;
  src: string;
  size?: AvatarSize;
  width?: string;
  height?: string;
  rounded?: Rounded;
  imgProps?: React.ComponentPropsWithoutRef<'img'>;
  textDivClassName?: React.ComponentPropsWithoutRef<'div'>['className'];
};

const Avatar = ({
  text,
  size = 'sm',
  rounded = 'circle',
  width,
  height,
  src,
  className,
  imgProps,
  textDivClassName,
  ...props
}: AvatarProps) => {
  const imageRef = useRef<React.LegacyRef<HTMLImageElement>>(null);
  const [hasImageLoaded, setImageLoaded] = useState(false);
  const [removeImage, setImageRemove] = useState(false);

  const handleImageLoad = () => {
    setImageLoaded(true);
  };

  const handleImageError = () => {
    setImageRemove(true);
  };

  return (
    <div
      className={clsx(
        `avatar-${size} me-2 d-flex align-items-center justify-content-center`,
        className
      )}
      style={{ width, height }}
      {...props}
    >
      {!removeImage && (
        <img
          ref={imageRef.current}
          src={src || ''}
          onLoad={handleImageLoad}
          onError={handleImageError}
          className={clsx(
            `rounded-${rounded} w-100 h-100`,
            !hasImageLoaded && 'hide-text'
          )}
          {...imgProps}
        />
      )}
      {!hasImageLoaded && (
        <div
          className={clsx(
            `rounded-${rounded} h-100 w-100 d-flex align-items-center justify-content-center`,
            textDivClassName || 'bg-light'
          )}
        >
          {text}
        </div>
      )}
    </div>
  );
};

export default Avatar;
