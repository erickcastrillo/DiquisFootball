import { Card } from 'react-bootstrap';
import clsx from 'clsx';

type StatisticsWidgetProps = {
  textClass?: string;
  bgClass?: string;
  icon?: string;
  title: string;
  description: string;
  stats?: string;
  trend: {
    textClass?: string;
    icon?: string;
    value?: string;
    time?: string;
  };
};

const StatisticsWidget = ({
  textClass,
  bgClass,
  icon,
  title,
  stats,
  trend,
  description
}: StatisticsWidgetProps) => {
  return (
    <Card className={clsx('widget-flat', bgClass)}>
      <Card.Body>
        {icon && (
          <div className="float-end">
            <i className={clsx(icon, 'widget-icon')}></i>
          </div>
        )}
        <h5
          className={clsx(
            'fw-normal',
            'mt-0',
            textClass ? textClass : 'text-muted'
          )}
          title={description}
        >
          {title}
        </h5>
        <h3 className={clsx('mt-3', 'mb-3', textClass ? textClass : null)}>
          {stats}
        </h3>

        {trend && (
          <p className={clsx('mb-0', textClass ? textClass : 'text-muted')}>
            <span className={clsx(trend.textClass, 'me-2')}>
              <i className={clsx(trend.icon)}></i> {trend.value}
            </span>
            <span className="text-nowrap">{trend.time}</span>
          </p>
        )}
      </Card.Body>
    </Card>
  );
};

export default StatisticsWidget;
