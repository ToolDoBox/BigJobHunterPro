const skeletonRows = Array.from({ length: 5 }, (_, index) => index);

export default function LoadingState() {
  return (
    <div className="space-y-6">
      <div className="hidden md:block space-y-3">
        {skeletonRows.map((row) => (
          <div
            key={row}
            className="h-12 rounded-lg bg-metal/60 animate-pulse motion-reduce:animate-none"
          />
        ))}
      </div>

      <div className="grid gap-4 md:hidden">
        {skeletonRows.slice(0, 3).map((row) => (
          <div
            key={row}
            className="h-24 rounded-lg bg-metal/60 animate-pulse motion-reduce:animate-none"
          />
        ))}
      </div>
    </div>
  );
}
