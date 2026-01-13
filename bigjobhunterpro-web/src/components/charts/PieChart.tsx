interface PieChartData {
  label: string;
  value: number;
  percentage: number;
}

interface PieChartProps {
  data: PieChartData[];
  title?: string;
  size?: number;
}

// Retro arcade color palette for pie slices
const COLORS = [
  '#FF6700', // Blaze Orange
  '#FFB000', // Amber
  '#00FF00', // Terminal Green
  '#2E4600', // Forest Green
  '#FF4444', // Red
  '#4444FF', // Blue
  '#FF44FF', // Magenta
  '#44FFFF', // Cyan
  '#FFFF44', // Yellow
  '#888888', // Gray
];

/**
 * Simple SVG-based pie chart component
 * Renders a donut-style pie chart with a retro arcade aesthetic
 */
export default function PieChart({ data, title, size = 200 }: PieChartProps) {
  if (!data || data.length === 0) {
    return (
      <div className="text-center text-gray-500 py-8">
        {title && <h3 className="font-arcade text-xs text-amber mb-2">{title}</h3>}
        <p className="text-sm">No data available</p>
      </div>
    );
  }

  const center = size / 2;
  const radius = size / 2 - 10;
  const innerRadius = radius * 0.6; // Donut hole

  // Calculate pie slices
  let currentAngle = -90; // Start at top
  const slices = data.map((item, index) => {
    const angle = (item.percentage / 100) * 360;
    const startAngle = currentAngle;
    const endAngle = currentAngle + angle;
    currentAngle = endAngle;

    // Convert to radians
    const startRad = (startAngle * Math.PI) / 180;
    const endRad = (endAngle * Math.PI) / 180;

    // Calculate outer arc points
    const x1 = center + radius * Math.cos(startRad);
    const y1 = center + radius * Math.sin(startRad);
    const x2 = center + radius * Math.cos(endRad);
    const y2 = center + radius * Math.sin(endRad);

    // Calculate inner arc points
    const x3 = center + innerRadius * Math.cos(endRad);
    const y3 = center + innerRadius * Math.sin(endRad);
    const x4 = center + innerRadius * Math.cos(startRad);
    const y4 = center + innerRadius * Math.sin(startRad);

    const largeArcFlag = angle > 180 ? 1 : 0;

    // Create path for donut slice
    const path = [
      `M ${x1} ${y1}`,
      `A ${radius} ${radius} 0 ${largeArcFlag} 1 ${x2} ${y2}`,
      `L ${x3} ${y3}`,
      `A ${innerRadius} ${innerRadius} 0 ${largeArcFlag} 0 ${x4} ${y4}`,
      'Z'
    ].join(' ');

    return {
      path,
      color: COLORS[index % COLORS.length],
      ...item
    };
  });

  return (
    <div className="space-y-4">
      {title && (
        <h3 className="font-arcade text-xs text-amber">{title}</h3>
      )}

      <div className="flex flex-col md:flex-row gap-6 items-center">
        {/* Pie Chart SVG */}
        <svg
          width={size}
          height={size}
          viewBox={`0 0 ${size} ${size}`}
          className="flex-shrink-0"
        >
          {slices.map((slice, index) => (
            <g key={index}>
              <path
                d={slice.path}
                fill={slice.color}
                stroke="#1a1a1a"
                strokeWidth="2"
                className="transition-opacity hover:opacity-80 cursor-pointer"
              >
                <title>{`${slice.label}: ${slice.value} (${slice.percentage.toFixed(1)}%)`}</title>
              </path>
            </g>
          ))}

          {/* Center hole with dark background */}
          <circle
            cx={center}
            cy={center}
            r={innerRadius - 2}
            fill="#1a1a1a"
          />
        </svg>

        {/* Legend */}
        <div className="flex-1 space-y-2">
          {slices.map((slice, index) => (
            <div key={index} className="flex items-center gap-3 text-sm">
              <div
                className="w-4 h-4 rounded-sm border border-gray-700"
                style={{ backgroundColor: slice.color }}
              />
              <div className="flex-1 min-w-0">
                <div className="truncate text-gray-200" title={slice.label}>
                  {slice.label}
                </div>
              </div>
              <div className="text-gray-400 font-mono text-xs whitespace-nowrap">
                {slice.value} ({slice.percentage.toFixed(1)}%)
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
