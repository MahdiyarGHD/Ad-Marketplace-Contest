import { memo, useEffect, useState, type ReactNode } from "react";
import Transition from "./Transition";
import useUIStore from "../stores/useUIStore";
import { createPortal } from "react-dom";
import { CircleAlertIcon } from "lucide-react";
import { invokeHapticFeedbackImpact } from "../utils/common";

function Toasts() {
	const { toasts } = useUIStore();

	return createPortal(
		<div className="Toasts">
			{toasts.map((toast) => {
				return (
					<Toast
						key={toast.id}
						id={toast.id!}
						icon={toast.icon}
						title={toast.title}
					/>
				);
			})}
		</div>,
		document.body,
	);
}

export function Toast({
	id,
	icon,
	title,
}: {
	id: number;
	icon?: ReactNode;
	title: string;
}) {
	const [show, setShow] = useState(true);
	const { removeToast } = useUIStore();

	useEffect(() => {
		invokeHapticFeedbackImpact("medium");
		setTimeout(() => {
			setShow(false);
		}, 2000);

		return () => {
			onDeactivate();
		};
	}, []);

	const onDeactivate = () => {
		removeToast(id);
	};

	return (
		<Transition state={show} onDeactivate={onDeactivate}>
			<div className="Toast">
				{icon ?? <CircleAlertIcon />}
				<span>{title}</span>
			</div>
		</Transition>
	);
}

export default memo(Toasts);
