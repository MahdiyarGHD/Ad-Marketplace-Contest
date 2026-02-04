import { memo, useEffect, useState, type ReactNode } from "react"
import Transition from "./Transition"
import useUIStore from "../stores/useUIStore"
import { createPortal } from "react-dom";

function Toasts() {
  const { toasts } = useUIStore();

  return createPortal(
		<div className="Toasts">
			{toasts.map((toast) => {
				return (
					<Toast key={toast.id} icon={toast.icon} title={toast.title} />
				);
			})}
		</div>, document.body)
}

export function Toast({ icon, title }: {
  icon: ReactNode,
  title: string,
}) {
    const [show, setShow] = useState(true)

    useEffect(() => {
        setTimeout(() => {
            setShow(false)
        }, 2000);
    }, [])

    return <Transition state={show}>
        <div className="Toast">
            {icon}
            <span>{title}</span>
        </div>
    </Transition>
}

export default memo(Toasts)
